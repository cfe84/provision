using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Provision {
    class TypeProvisioningException: Exception {
        public TypeProvisioningException(string message): base(message) {}
    }
    static class Resolver {
        public class ResourceProperty {
            public string Name {get; set;}
            public string Description {get; set;}
            public string Type {get; set;}
        }

        public class KnownResourceType {
            public IReference Reference {get; set;}
            public string Description {get; set;}
            public Type Type {get;set;}
            public IEnumerable<ResourceProperty> Properties {get; set;}
        }

        private static string formatResourceTypeName(string resourceTypeName) {
            var regex = new Regex("([A-Z][a-z]+)");
            var formattedName = string.Join(" ", regex.Matches(resourceTypeName).Select(nameComponent => nameComponent.Value.ToLower()));
            return formattedName;  
        }

        private static string formatPropertyType(PropertyInfo property) => 
              property.PropertyType == typeof(string) ? "string"
            : property.PropertyType == typeof(string[]) ? "space-separated list"
            : formatResourceTypeName(property.PropertyType.Name) + " name";

        private static IEnumerable<ResourceProperty> discoverResourceProperties(Type t) =>
            t.GetProperties()
                .Where(DependencyUtils.IsUserFacingProperty)
                .Select(property => {
                    var dependencyAttribute = DependencyUtils.GetDependencyAttribute(property);
                    return new ResourceProperty() {
                        Name = property.Name,
                        Type = formatPropertyType(property),
                        Description = dependencyAttribute?.Description
                    };
                });

        private static Dictionary<Type, KnownResourceType> discoverResourceTypes() {
            var assembly = Assembly.GetExecutingAssembly();
            Func<Type, bool> isAResourceGenerator = (Type t) => 
                t.GetInterfaces().Any(interf => interf == typeof(IResourceGenerator));
                
            Func<Type, KeyValuePair<Type, ConstructorInfo>> eligibleConstructor = (Type t) =>
                t.GetConstructors()
                    .Where(constructor =>
                        constructor.GetParameters()
                            .Count() == 1 &&
                        constructor.GetParameters()
                            .All(param => param.ParameterType.GetInterfaces()
                                .Any(interf => interf == typeof(IResource))))
                    .Select(constructor => new KeyValuePair<Type, ConstructorInfo>(constructor.GetParameters()[0].ParameterType, constructor))
                    .FirstOrDefault();

            return assembly.GetTypes()
                .Where(isAResourceGenerator)
                .Select(type => eligibleConstructor(type))
                .ToDictionary(
                    kv => kv.Key,
                    kv => {
                        var resourceAttribute = DependencyUtils.GetResourceAttribute(kv.Key);
                        return new KnownResourceType{
                            Type = kv.Key,
                            Description = resourceAttribute?.Description,
                            Reference = (IReference)new GenericReference(kv.Key, kv.Value),
                            Properties = discoverResourceProperties(kv.Key)
                        };
                    });
        }

        private static Dictionary<Type, KnownResourceType> knownResourceTypes = discoverResourceTypes();

        public static IReference GetReference(Type resourceType) {
            if (knownResourceTypes.ContainsKey(resourceType)) {
                return knownResourceTypes[resourceType].Reference;
            }
            throw new TypeProvisioningException($"Unknown resource type: {resourceType.Name}");
        }

        public static IReference GetReference(string resourceName) {
            var reference = knownResourceTypes.FirstOrDefault(
                kv => kv.Key.Name.Equals(resourceName, StringComparison.InvariantCultureIgnoreCase));
            if (reference.Value == null) {
                throw new TypeProvisioningException($"Unknown resource type: {resourceName}");
            }
            return reference.Value.Reference;
        }

        public static IResourceGenerator GetResourceGenerator(IResource resource) {
            var reference = GetReference(resource.GetType());
            return reference.CreateResourceGenerator(resource);
        }

        public static IEnumerable<KnownResourceType> KnownResourceTypes { get => knownResourceTypes.Values; }
    }
}
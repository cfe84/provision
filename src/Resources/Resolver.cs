using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Provision {
    class TypeProvisioningException: Exception {
        public TypeProvisioningException(string message): base(message) {}
    }
    static class Resolver {
        private static Dictionary<Type, IReference> findResourceTypes() {
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
                .ToDictionary(kv => kv.Key, kv => (IReference)new GenericReference(kv.Key, kv.Value));
        }

        private static Dictionary<Type, IReference> KnownTypes = findResourceTypes();

        public static IReference GetReference(Type resourceType) {
            if (KnownTypes.ContainsKey(resourceType)) {
                return KnownTypes[resourceType];
            }
            throw new TypeProvisioningException($"Unknown resource type: {resourceType.Name}");
        }

        public static IReference GetReference(string resourceName) {
            var reference = KnownTypes.FirstOrDefault(
                kv => kv.Key.Name.Equals(resourceName, StringComparison.InvariantCultureIgnoreCase));
            if (reference.Value == null) {
                throw new TypeProvisioningException($"Unknown resource type: {resourceName}");
            }
            return reference.Value;
        }

        public static IResourceGenerator GetResourceGenerator(IResource resource) {
            var reference = GetReference(resource.GetType());
            return reference.CreateResourceGenerator(resource);
        }
    }
}
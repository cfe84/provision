using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Provision {
    class DefaultResourceParser<T>: IResourceParser where T: IResource
    {
        private IResourceParser parser = new DefaultResourceParser(typeof(T));
        public ParsedResource ParseResourceSpecification(Context context, ResourceSpecification specification)
            => parser.ParseResourceSpecification(context, specification);
    }
    class DefaultResourceParser : IResourceParser
    {
        public DefaultResourceParser(Type t)
        {
            type = t;
            properties = type.GetProperties();
        }
        Type type;
        PropertyInfo[] properties;

        public ParsedResource ParseResourceSpecification(Context context, ResourceSpecification resourceSpecification)
        {
            IResource result = InstantiateResourceObject(context);
            var properties = type.GetProperties();
            var dependencies = ProcessStringProperties(resourceSpecification, result);
            ProcessListProperties(resourceSpecification, result);
            return new ParsedResource {
                Resource = result,
                ExplicitDependenciesToBeInjected = dependencies
            };
        }

        private IResource InstantiateResourceObject(Context context)
        {
            var constructor = type.GetConstructor(new[] { typeof(Context) });
            IResource result = (IResource)constructor.Invoke(new object[] { context });
            return result;
        }

        private string CleanupName(string name) => name.Replace("-", "");

        private PropertyInfo FindMatchingProperty(string propertyName)
        {
            propertyName = CleanupName(propertyName);
            return properties.FirstOrDefault(
                p => (p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
            );
        }

        private void ProcessListProperties(ResourceSpecification resourceSpecification, IResource resultResource)
        {
            foreach (var kv in resourceSpecification.ListProperties)
            {
                var property = FindMatchingProperty(kv.Key);
                if (property != null)
                {
                    if (property.PropertyType == typeof(string[])) {
                        property.SetValue(resultResource, kv.Value);
                    } 
                    else if (property.PropertyType == typeof(string)) {
                        property.SetValue(resultResource, string.Join(" ", kv.Value));
                    }
                    else {
                        throw new ParserException($"In {type.Name}: trying to assign a list to a property of type {property.PropertyType}");
                    }
                }
                else
                {
                    throw new ParserException($"Unknown {type.Name} list property: {kv.Key}");
                }
            }
        }

        private static bool isAResource(Type t) => 
            t.GetInterfaces().Any(interf => interf == typeof(IResource));

        private IEnumerable<DependencyRequirement> ProcessStringProperties(ResourceSpecification resourceSpecification, IResource resultResource)
        {
            foreach (var kv in resourceSpecification.StringProperties)
            {
                PropertyInfo property = FindMatchingProperty(kv.Key);
                if (property != null)
                {
                    if (property.PropertyType == typeof(string)) {
                        property.SetValue(resultResource, kv.Value);
                    }
                    else if (property.PropertyType == typeof(Int32) || property.PropertyType == typeof(Int64) || property.PropertyType == typeof(int)) {
                        property.SetValue(resultResource, int.Parse(kv.Value));
                    }
                    else if (property.PropertyType == typeof(string[])) {
                        property.SetValue(resultResource, new[] { kv.Value });
                    }
                    else if (DependencyUtils.PropertyIsResource(property)) {
                        yield return new DependencyRequirement() {
                            Property = property,
                            ValueName = kv.Value
                        };
                    }
                    else {
                        throw new ParserException($"Incorrect property type for property {kv.Key}: {property.PropertyType}");
                    }
                }
                else
                {
                    throw new ParserException($"Property not found: {kv.Key}");
                }
            }
        }
    }
}
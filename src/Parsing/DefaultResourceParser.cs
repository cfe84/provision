using System;
using System.Linq;
using System.Reflection;

namespace Provision {
    class DefaultResourceParser<T>: IResourceParser where T: IResource
    {
        private IResourceParser parser = new DefaultResourceParser(typeof(T));
        public IResource ParseResourceSpecification(Context context, ResourceSpecification specification)
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

        public IResource ParseResourceSpecification(Context context, ResourceSpecification resourceSpecification)
        {
            IResource result = InstantiateObject(context);
            var properties = type.GetProperties();
            ProcessStringProperties(resourceSpecification, result);
            ProcessListProperties(resourceSpecification, result);
            return result;
        }

        private PropertyInfo FindMatchingProperty(string propertyName, Type type)
        {
            return properties.FirstOrDefault(
                                p => (p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase) ||
                                    p.Name.Equals(propertyName + "name", StringComparison.InvariantCultureIgnoreCase)) &&
                                    p.PropertyType == type);
        }

        private void ProcessListProperties(ResourceSpecification resourceSpecification, IResource resultResource)
        {
            foreach (var kv in resourceSpecification.ListProperties)
            {
                var property = FindMatchingProperty(kv.Key, typeof(string[]));
                if (property != null)
                {
                    property.SetValue(resultResource, kv.Value);
                }
                else
                {
                    throw new Exception($"Unknown {type.Name} list property: {kv.Key}");
                }
            }
        }


        private void ProcessStringProperties(ResourceSpecification resourceSpecification, IResource resultResource)
        {
            foreach (var kv in resourceSpecification.StringProperties)
            {
                PropertyInfo property = FindMatchingProperty(kv.Key, typeof(string));
                var dependency = resultResource.DependencyRequirements.FirstOrDefault(
                    d => d.Name.Equals(kv.Key, StringComparison.InvariantCultureIgnoreCase));
                if (property != null)
                {
                    property.SetValue(resultResource, kv.Value);
                }
                else if (dependency != null)
                {
                    DependencyUtils.SetDependencyValueName(
                        resultResource.DependencyRequirements,
                        dependency.Type,
                        dependency.Name,
                        kv.Value);
                }
                else
                {
                    throw new Exception($"Unknown {type.Name} property: {kv.Key}");
                }
            }
        }


        private IResource InstantiateObject(Context context)
        {
            var constructor = type.GetConstructor(new[] { typeof(Context) });
            IResource result = (IResource)constructor.Invoke(new object[] { context });
            return result;
        }
    }
}
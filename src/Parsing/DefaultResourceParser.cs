using System;
using System.Linq;
using System.Reflection;

namespace Provision {
    class DefaultResourceParser<T> : IResourceParser where T: IResource
    {
        Type type = typeof(T);
        PropertyInfo[] properties = (typeof(T)).GetProperties();

        public IResource ParseResourceSpecification(Context context, ResourceSpecification resourceSpecification)
        {
            T result = InstantiateObject(context);
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

        private void ProcessListProperties(ResourceSpecification resourceSpecification, T result)
        {
            foreach (var kv in resourceSpecification.ListProperties)
            {
                var property = FindMatchingProperty(kv.Key, typeof(string[]));
                if (property != null)
                {
                    property.SetValue(result, kv.Value);
                }
                else
                {
                    throw new Exception($"Unknown {type.Name} list property: {kv.Key}");
                }
            }
        }


        private void ProcessStringProperties(ResourceSpecification resourceSpecification, T result)
        {
            foreach (var kv in resourceSpecification.StringProperties)
            {
                PropertyInfo property = FindMatchingProperty(kv.Key, typeof(string));
                var dependency = result.DependencyRequirements.FirstOrDefault(
                    d => d.Name.Equals(kv.Key, StringComparison.InvariantCultureIgnoreCase));
                if (property != null)
                {
                    property.SetValue(result, kv.Value);
                }
                else if (dependency != null)
                {
                    DependencyUtils.SetDependencyValueName(
                        result.DependencyRequirements,
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


        private T InstantiateObject(Context context)
        {
            var constructor = type.GetConstructor(new[] { typeof(Context) });
            T result = (T)constructor.Invoke(new object[] { context });
            return result;
        }
    }
}
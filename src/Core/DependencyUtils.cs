using System;
using System.Linq;
using System.Reflection;

namespace Provision
{
    class DependencyUtils {
        
        public static DependencyAttribute GetDependencyAttribute(PropertyInfo property) =>
            (DependencyAttribute)Attribute.GetCustomAttribute(property, typeof(DependencyAttribute));
        public static ResourceAttribute GetResourceAttribute(Type type) =>
            (ResourceAttribute)Attribute.GetCustomAttribute(type, typeof(ResourceAttribute));

        public static bool PropertyIsResource(PropertyInfo property) =>
            property.PropertyType.GetInterfaces().Any(interf => interf == typeof(IResource));
        public static bool PropertiesHasSetter(PropertyInfo property) =>
            property.SetMethod != null;

        public static bool IsUserFacingProperty(PropertyInfo property) =>
            PropertiesHasSetter(property) &&
            (   property.PropertyType == typeof(string) ||
                property.PropertyType == typeof(string[]) ||
                PropertyIsResource(property));
                
    }
}
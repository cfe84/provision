using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Provision {

    internal class DependencyResolutionException : System.Exception
    {
        public DependencyResolutionException(string message) : base(message) { }
    }

    internal class Injector {
        private readonly Context context;

        public Injector(Context context)
        {
            this.context = context;
        }
        
        public void Inject() {
            var initialResources = context.Resources.ToArray();
            foreach(IResource resource in initialResources)
            {
                InjectDependenciesForResource(resource);
            }
        }

        private IResource CreateDefaultDependency(Type type) {
            IResource resource = null;
            var constructor = type.GetConstructor(new [] { typeof(Context) });
            if (constructor == null) {
                throw new DependencyResolutionException($"{type} doesn't have a default implementation");
            }
            resource = (IResource)constructor.Invoke(new object[] { context });
            context.Resources.Add(resource);
            InjectDependenciesForResource(resource);
            return resource;
        }

        private IResource GetDefaultDependency(Type type) {
            var correspondingDependency = context.Resources.FirstOrDefault(resource => resource.GetType() == type);
            if (correspondingDependency == null) {
                return CreateDefaultDependency(type);
            }
            else {
                return correspondingDependency;
            }
        }

        private static DependencyAttribute getDependencyAttribute(PropertyInfo property) =>
            (DependencyAttribute)Attribute.GetCustomAttribute(property, typeof(DependencyAttribute));


        private static bool propertyIsResource(PropertyInfo property) =>
            property.PropertyType.GetInterfaces().Any(interf => interf == typeof(IResource));
        private static bool propertiesHasSetter(PropertyInfo property) =>
            property.SetMethod != null;

        private IEnumerable<DependencyRequirement> getResourceDependencyRequirements(IResource resource) {
            var requirements =
                context.ExplicitDependencyRequirements.ContainsKey(resource) ?
                context.ExplicitDependencyRequirements[resource] :
                new List<DependencyRequirement>();
            var propertiesWithTypeResourceAndSetters = resource.GetType().GetProperties()
                .Where(propertyIsResource)
                .Where(propertiesHasSetter);
            foreach(var property in propertiesWithTypeResourceAndSetters) {
                var dependencyAttribute = getDependencyAttribute(property);
                var propertyIsAlreadyInExplicitRequirements = requirements.Any(requirement => requirement.Property.Name == property.Name);
                var propertyIsAnOptionalDependency = dependencyAttribute != null ? dependencyAttribute.Optional : false;
                if (!propertyIsAlreadyInExplicitRequirements && 
                    !propertyIsAnOptionalDependency) {
                        var missingDependency = new DependencyRequirement {
                            Property = property,
                            ValueName = "default"
                        };
                        requirements.Add(missingDependency);
                }
            }
            return requirements;
        }

        private static void InjectDependency(IResource resource, DependencyRequirement dependencyRequirement, IResource correspondingDependency)
        {
            var resourceType = resource.GetType();
            dependencyRequirement.Property.SetValue(resource, correspondingDependency);
        }

        private void InjectDependenciesForResource(IResource resource)
        {
            foreach (var dependencyRequirements in getResourceDependencyRequirements(resource))
            {
                var correspondingDependencies = context.Resources.Where(
                    (candidate) => candidate.GetType() == dependencyRequirements.Property.PropertyType
                        && candidate.Name == dependencyRequirements.ValueName);
                var count = correspondingDependencies.Count();
                IResource correspondingDependency;
                if (count > 1)
                {
                    throw new DependencyResolutionException(
                        $"Expected exactly one match for {dependencyRequirements.Property.PropertyType}:{dependencyRequirements.ValueName} "
                        + $"but found {correspondingDependencies.Count()}");
                }
                if (count == 0)
                    correspondingDependency = GetDefaultDependency(dependencyRequirements.Property.PropertyType);
                else
                    correspondingDependency = correspondingDependencies.First();
                InjectDependency(resource, dependencyRequirements, correspondingDependency);
            }
        }

    }    
}
using System;
using System.Linq;

namespace Provision {

    [System.Serializable]
    internal class DependencyResolutionException : System.Exception
    {
        public DependencyResolutionException(string message) : base(message) { }
    }
    internal static class Injector {
        private static IResource CreateDefaultDependency(Context context, Type type) {
            IResource resource = null;
            var constructor = type.GetConstructor(new [] { typeof(Context) });
            if (constructor == null) {
                throw new DependencyResolutionException($"{type} doesn't have a default implementation");
            }
            resource = (IResource)constructor.Invoke(new object[] { context });
            context.Resources.Add(resource);
            InjectDependenciesForResource(context, resource);
            return resource;
        }

        private static IResource GetDefaultDependency(Context context, Type type) {
            var correspondingDependency = context.Resources.FirstOrDefault(resource => resource.GetType() == type);
            if (correspondingDependency == null) {
                return CreateDefaultDependency(context, type);
            }
            else {
                return correspondingDependency;
            }
        }

        public static void Inject(Context context) {
            var initialResources = context.Resources.ToArray();
            foreach(IResource resource in initialResources)
            {
                InjectDependenciesForResource(context, resource);
            }
        }

        private static void InjectDependenciesForResource(Context context, IResource resource)
        {
            foreach (DependencyRequirement dependencyRequirements in resource.DependencyRequirements)
            {
                var correspondingDependencies = context.Resources.Where(
                    (candidate) => candidate.GetType() == dependencyRequirements.Type
                        && candidate.Name == dependencyRequirements.ValueName);
                var count = correspondingDependencies.Count();
                IResource correspondingDependency;
                if (count > 1)
                {
                    throw new DependencyResolutionException(
                        $"Expected exactly one match for {dependencyRequirements.Type}:{dependencyRequirements.ValueName} "
                        + $"but found {correspondingDependencies.Count()}");
                }
                if (count == 0)
                    correspondingDependency = GetDefaultDependency(context, dependencyRequirements.Type);
                else
                    correspondingDependency = correspondingDependencies.First();
                InjectDependency(resource, dependencyRequirements, correspondingDependency);
            }
        }

        private static void InjectDependency(IResource resource, DependencyRequirement dependencyRequirements, IResource correspondingDependency)
        {
            var resourceType = resource.GetType();
            var property = resourceType.GetProperty(dependencyRequirements.Name);
            property.SetValue(resource, correspondingDependency);
        }
    }    
}
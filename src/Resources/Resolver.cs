using System;
using System.Collections.Generic;
using System.Linq;

namespace Provision {
    class TypeProvisioningException: Exception {
        public TypeProvisioningException(string message): base(message) {}
    }
    static class Resolver {
        private static Dictionary<Type, IReference> KnownTypes = new Dictionary<Type, IReference> {
            [typeof(StorageAccount)] = new StorageAccountReference(),
            [typeof(ResourceGroup)] = new ResourceGroupReference(),
            [typeof(AppServicePlan)] = new AppServicePlanReference(),
            [typeof(WebApp)] = new WebAppReference(),
        };

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
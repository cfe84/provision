using System;
using System.Collections.Generic;

namespace Provision {

    public class ParserException: Exception {
        public ParserException(string message): base(message) {}
    }

    internal static class Parser {
        private static IResource ParseStorageAccount(Context context, ResourceSpecification resourceSpecification) {
            var storageAccount = new StorageAccount(context);
            foreach(var kv in resourceSpecification.StringProperties) {
                switch(kv.Key.ToLower()) {
                    case "name": 
                        storageAccount.Name = kv.Value;
                        break;
                    case "resourcegroup":
                        DependencyUtils.SetDependencyValueName(
                            storageAccount.dependencyRequirements,
                            typeof(ResourceGroup),
                            "ResourceGroup",
                            kv.Value);
                        break;
                    case "location":
                        storageAccount.Location = kv.Value;
                        break;
                    case "postfix":
                        storageAccount.AccountPostfix = kv.Value;
                        break;
                    default:
                        throw new Exception("Unknown storage account property: " + kv.Key);
                }
            }
            foreach(var kv in resourceSpecification.ListProperties) {
                switch(kv.Key.ToLower()) {
                    case "containers":
                        storageAccount.Containers = kv.Value.ToArray();
                        break;
                    default:
                        throw new Exception("Unknown storage account list property: " + kv.Key);
                }
            }
            return storageAccount;
        }

        private static IResource ParseResourceGroup(Context context, ResourceSpecification resourceSpecification) {
            var resourceGroup = new ResourceGroup(context);
            foreach(var kv in resourceSpecification.StringProperties) {
                switch(kv.Key.ToLower()) {
                    case "name":
                        resourceGroup.Name = kv.Value;
                        break;
                    case "resourcegroupname":
                    case "value":
                        resourceGroup.ResourceGroupName = kv.Value;
                        break;
                    case "location":
                        resourceGroup.Location = kv.Value;
                        break;
                    default:
                        throw new Exception("Unknown resource group property: " + kv.Key);
                }
            }
            foreach(var kv in resourceSpecification.ListProperties) {
                throw new Exception("Unknown resource group list property: " + kv.Key);
            }
            return resourceGroup;
        }

        public static Context Parse(ResourceTree resourceTree) {
            var context = new Context();
            if (resourceTree != null)
                context.DefaultLocation = resourceTree.Location;

            foreach(ResourceSpecification resourceSpecification in resourceTree.Resources) {
                IResource resource;
                switch(resourceSpecification.ResourceType.ToLower()) {
                    case "storageaccount":
                        resource = ParseStorageAccount(context, resourceSpecification);
                        break;
                    case "resourcegroup":
                        resource = ParseResourceGroup(context, resourceSpecification);
                        break;
                    default:
                        throw new ParserException($"Unknown resource type: {resourceSpecification.ResourceType}");
                }
                context.Resources.Add(resource);
            }

            return context;
        }
    }
}
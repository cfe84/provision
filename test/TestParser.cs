using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Provision.Test {
    public class TestParser {
        [Fact]
        public void Parser_should_parse() {
            // Prepare
            var tree = new ResourceTree();
            tree.Location = "myLocation";
            var resources = new List<ResourceSpecification>();
            var storageAccount = new ResourceSpecification {
                ResourceType = "StorageAccount"
                };
            storageAccount.ListProperties.Add("containers", new List<string>{"abc"});
            storageAccount.StringProperties.Add("ResourceGroup", "someGroup");
            resources.Add(storageAccount);
            var resourceGroup = new ResourceSpecification {
                ResourceType = "ResourceGroup",
            };
            resourceGroup.StringProperties.Add("Location", "otherLocation");
            resources.Add(resourceGroup);
            tree.Resources = resources;

            // Execute
            var context = Parser.Parse(tree);

            // Assess
            Assert.Equal(tree.Location, context.DefaultLocation);
            Assert.Single(context.Resources, resource => 
                resource.GetType() == typeof(StorageAccount) &&
                ((StorageAccount)resource).Containers.Length == 1 &&
                ((StorageAccount)resource).Containers[0] == "abc" &&
                ((StorageAccount)resource).DependencyRequirements.Any(
                    req => req.Type == typeof(ResourceGroup) && 
                    req.Name == "ResourceGroup" && 
                    req.ValueName == "someGroup")
            );
            
            Assert.Single(context.Resources, resource => 
                resource.GetType() == typeof(ResourceGroup) &&
                ((ResourceGroup)resource).Location == "otherLocation"
            );
        }
    }
}
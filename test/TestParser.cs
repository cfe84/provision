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
            storageAccount.ListProperties.Add("containers", new []{"abc"});
            storageAccount.StringProperties.Add("queues", "queueName");
            storageAccount.StringProperties.Add("ResourceGroup", "someGroup");
            storageAccount.ListProperties.Add("name", new string[] { "a", "name"});
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
            var parsedStorageAccount = context.Resources.FirstOrDefault(resource => 
                resource.GetType() == typeof(StorageAccount) &&
                resource.Name == "a name" &&
                ((StorageAccount)resource).Containers.Length == 1 &&
                ((StorageAccount)resource).Containers[0] == "abc" &&
                ((StorageAccount)resource).Queues.Length == 1 &&
                ((StorageAccount)resource).Queues[0] == "queueName"
                );
            Assert.NotNull(parsedStorageAccount);

            Assert.Single(context.ExplicitDependencyRequirements[parsedStorageAccount],
                req => 
                    req.Property.PropertyType == typeof(ResourceGroup) && 
                    req.Property.Name == "ResourceGroup" && 
                    req.ValueName == "someGroup");
            
            Assert.Single(context.Resources, resource => 
                resource.GetType() == typeof(ResourceGroup) &&
                ((ResourceGroup)resource).Location == "otherLocation"
            );
        }
        [Fact]
        public void Parser_should_parse_ints() {
            // Prepare
            var tree = new ResourceTree();
            var resources = new List<ResourceSpecification>();
            var snippet = new ResourceSpecification {
                ResourceType = "Snippet",
                };
            snippet.StringProperties.Add("order", "3");
            resources.Add(snippet);
            tree.Resources = resources;

            // Execute
            var context = Parser.Parse(tree);

            // Assess
            var parsedSnippet = context.Resources.FirstOrDefault(resource => 
                resource.GetType() == typeof(Snippet));
            Assert.NotNull(parsedSnippet);
            Assert.Equal(3, parsedSnippet.Order);
        }

    }
}
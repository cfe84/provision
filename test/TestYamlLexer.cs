using System.Linq;
using Xunit;

namespace Provision.Test {
    public class TestYamlLexer {
        
        [Fact]
        public void ShouldLoad() {
            // execute
            var tree = YamlLexer.LoadResourcesFromString(EXAMPLE_FILE);

            // Assert
            Assert.Equal("canadacentral", tree.Location);
            Assert.Equal(2, tree.Resources.Count());
            Assert.Single(tree.Resources, resource => 
                resource.ResourceType == "resourceGroup"
                && resource.StringProperties["name"] == "defaultrg"
                && resource.StringProperties["value"] == "$NAME");
            Assert.Single(tree.Resources, resource =>
                resource.ResourceType == "storageAccount"
                && resource.StringProperties["resourceGroup"] == "defaultrg"
                && resource.ListProperties["containers"].Count == 1
                && resource.ListProperties["containers"][0] == "testData");        
        }

        const string EXAMPLE_FILE = @"deployment:
  location: ""canadacentral""
  resources:
  - type: ""resourceGroup""
    name: ""defaultrg""
    value: ""$NAME""
  - type: ""storageAccount""
    resourceGroup: ""defaultrg""
    containers:
    - ""testData"" ";
    }

    
}
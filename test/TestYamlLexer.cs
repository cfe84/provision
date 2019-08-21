using System.Linq;
using Xunit;

namespace Provision.Test
{
    public class TestYamlLexer
    {

        [Fact]
        public void ShouldLoad()
        {
            // execute
            var tree = YamlLexer.LoadResourcesFromString(EXAMPLE_FILE);

            // Assert
            Assert.Equal("canadacentral", tree.Location);
            Assert.Equal(3, tree.Resources.Count());
            Assert.Single(tree.Resources, resource =>
                resource.ResourceType == "resourceGroup"
                && resource.StringProperties["name"] == "defaultrg"
                && resource.StringProperties["value"] == "$NAME");
            Assert.Single(tree.Resources, resource =>
                resource.ResourceType == "storageAccount"
                && resource.StringProperties["resourceGroup"] == "defaultrg"
                && resource.ListProperties["containers"].Length == 1
                && resource.ListProperties["containers"][0] == "testData");
            Assert.Single(tree.Resources, resource =>
                resource.ResourceType == "serviceBus"
                && resource.StringProperties["name"] == "the value");
        }

        const string EXAMPLE_FILE = @"deployment:
  location: ""canadacentral""
  resources:
  - type: ""resource-group""
    name: ""defaultrg""
    value: ""$NAME""
  - type: ""Service bus""
    name: ""the value""
  - type: ""storageAccount""
    resourceGroup: ""defaultrg""
    containers:
    - ""testData"" ";
    }


}
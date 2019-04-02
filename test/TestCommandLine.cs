using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Xunit;

namespace Provision.Test
{
    public class TestCommandLine
    {
        [Fact]
        public void CommandLineParser_should_parse()
        {
            //prepare
            var commandLine = new [] { 
                "--location", "somewhere",
                "storageAccount", "--containers", "cont1 cont2 cont3", "--resourceGroup", "default",
                "resourceGroup", "--location", "eastus"};

            // execute
            var tree = CommandLineLexer.LexCommandLine(commandLine);

            // assess
            Assert.Equal("somewhere", tree.Location);
            Assert.Equal(2, tree.Resources.Count());
            Assert.Single(tree.Resources, 
                resource => resource.ResourceType == "storageAccount" &&
                resource.StringProperties["resourceGroup"] == "default" &&
                resource.ListProperties["containers"].Count() == 3 &&
                resource.ListProperties["containers"].Contains("cont2"));

            Assert.Single(tree.Resources,
                resource => resource.ResourceType == "resourceGroup" &&
                resource.StringProperties["location"] == "eastus");
        }
    }
}
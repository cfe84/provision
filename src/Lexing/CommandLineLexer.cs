using System;
using System.Collections.Generic;

namespace Provision {
    internal static class CommandLineLexer {

        private static bool isResource(string component) {
            return component.Substring(0, 2) != "--";
        }

        private static bool isOption(string component) {
            return component.Substring(0, 2) == "--";
        }

        private static string getOptionName(string component) 
            => component.Substring(2);

        private static bool isList(string component) {
            return component.Contains(" ");
        }

        private static string[] parseList(string component) {
            return component.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        }

        internal static ResourceTree LexCommandLine(string[] commandLine) {
            var tree = new ResourceTree();
            var index = 0;
            ResourceSpecification resource = null;
            List<ResourceSpecification> resources = new List<ResourceSpecification>();
            tree.Resources = resources;

            while (index < commandLine.Length) {
                if (isResource(commandLine[index])) {
                    resource = new ResourceSpecification() {
                        ResourceType = commandLine[index]
                    };
                    resources.Add(resource);
                } 
                else if (isOption(commandLine[index])) {
                    var optionName = getOptionName(commandLine[index]);
                    index++;
                    if (!isList(commandLine[index])) {
                        if (optionName == "location" && resource == null) {
                            tree.Location = commandLine[index];
                        } else {
                            resource.StringProperties.Add(optionName, commandLine[index]);
                        }
                    } else {
                        resource.ListProperties.Add(optionName, parseList(commandLine[index]));
                    }
                }
                index++;
            }
            return tree;
        }
    }    
}
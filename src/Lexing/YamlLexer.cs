using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace Provision
{
    class YamlLexingException : FunctionalException
    {
        public YamlLexingException(string message) : base(message) { }
    }

    internal static class YamlLexer
    {
        private static string standardizeResourceType(string resourceType)
        {
            var split = resourceType.Replace("-", " ").Split(" ");
            if (split.Length == 1) return split[0];
            split[0] = split[0].ToLower();
            for (int i = 1; i < split.Length; i++)
            {
                split[i] = split[i].Substring(0, 1).ToUpper() + split[i].Substring(1).ToLower();
            }
            return string.Concat(split);
        }

        public static ResourceTree LoadResourcesFromFile(string filePath)
        {
            var file = File.ReadAllText(filePath);
            return LoadResourcesFromString(file);
        }
        public static ResourceTree LoadResourcesFromString(string yamlContent)
        {
            var result = new ResourceTree();
            var resources = new List<ResourceSpecification>();
            result.Resources = resources;

            var reader = new StringReader(yamlContent);
            var yaml = new YamlStream();
            yaml.Load(reader);
            var document = yaml.Documents[0];
            var rootNode = ((YamlMappingNode)((YamlMappingNode)document.RootNode).Children["deployment"]);

            if (rootNode.Children.Keys.Contains("location"))
                result.Location = rootNode.Children["location"].ToString();

            foreach (var resourceNode in (YamlSequenceNode)rootNode.Children["resources"])
            {
                var resource = new ResourceSpecification();
                foreach (var attributeNode in ((YamlMappingNode)resourceNode).Children)
                {
                    if (((YamlScalarNode)attributeNode.Key).Value == "type")
                    {
                        resource.ResourceType = standardizeResourceType(((YamlMappingNode)resourceNode).Children["type"].ToString());
                    }
                    else if (attributeNode.Value.NodeType == YamlNodeType.Scalar)
                    {
                        resource.StringProperties.Add(
                            ((YamlScalarNode)attributeNode.Key).Value,
                            ((YamlScalarNode)attributeNode.Value).Value);
                    }
                    else if (attributeNode.Value.NodeType == YamlNodeType.Sequence)
                    {
                        resource.ListProperties.Add(
                            ((YamlScalarNode)attributeNode.Key).Value,
                            ((YamlSequenceNode)attributeNode.Value).Children
                                .Select(listElement => listElement.ToString())
                                .ToArray()
                            );
                    }
                    else
                    {
                        throw new YamlLexingException($"Don't know what to do with element {attributeNode.Key} of type {attributeNode.Value.NodeType}");
                    }
                }
                if (resource.ResourceType == null)
                {
                    throw new YamlLexingException($"Missing a resource type at line {resourceNode.Start.Line}:{resourceNode.Start.Column}");
                }
                resources.Add(resource);
            }
            return result;
        }
    }
}
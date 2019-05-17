using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("Provision.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Provision
{
    class Program
    {
        const int PROPERTY_DESCRIPTION_ALIGNMENT_LEFT = 45;
        static string FormatProperty(Resolver.ResourceProperty property) {
            var regex = new Regex("([A-Z]+[a-z]*)");
            var formattedName = string.Join("", regex.Matches(property.Name).Select(nameComponent => (nameComponent.Index == 0 ? "--" : "-") + nameComponent.Value.ToLower()));
            var nameAndParameterType = formattedName + " (" + property.Type + ")";
            var leftSpaces = "        ";
            var spacesBetweenNameAndDescription = "";
            for (int i = 0; i < PROPERTY_DESCRIPTION_ALIGNMENT_LEFT - nameAndParameterType.Length; i++) {
                spacesBetweenNameAndDescription += " ";
            }
            return leftSpaces + nameAndParameterType + spacesBetweenNameAndDescription + " " + property.Description;
        }

        static void Usage(string name) {
            var types = Resolver.KnownResourceTypes;
            var usage = string.Join("\n", types.Select(type => 
                type.Type.Name + "\n" +
                string.Join("\n", type.Properties.Select(FormatProperty))
            ));
            Console.WriteLine($"Usage: {name} [Resources to create] [-f file.yml]\n\n\n{usage}\n\n");
        }

        static void Main(string[] args)
        {
            try {
                ResourceTree tree;
                if (args.Length == 0 || args[0] == "-h") {
                    Usage(System.AppDomain.CurrentDomain.FriendlyName);
                    return;
                } 
                else if (args[0] == "-f") {
                    var file = args[1];
                    tree = YamlLexer.LoadResourcesFromFile(file);
                }
                else {
                    tree = CommandLineLexer.LexCommandLine(args);
                }
                var context = Parser.Parse(tree);
                var injector = new Injector(context);
                injector.Inject();
                var generate = new Generate(context);
                Console.WriteLine(generate.BuildString());
            }
            catch (Exception exception) {
                if (exception.GetType().IsSubclassOf(typeof(FunctionalException))) {
                    Console.Error.WriteLine($"{exception.GetType().Name}: {exception.Message}");
                } 
                else if (exception.InnerException != null && exception.InnerException.GetType().IsSubclassOf(typeof(FunctionalException))) {
                    Console.Error.WriteLine($"{exception.InnerException.GetType().Name}: {exception.InnerException.Message}");
                }
                else {
                    throw exception;
                }
            }
        }
    }
}

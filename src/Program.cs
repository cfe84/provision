using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Provision.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Provision
{
    class Program
    {
        static void Main(string[] args)
        {
            ResourceTree tree;
            if (args[0] == "-f") {
                var file = args[1];
                tree = YamlLexer.LoadResourcesFromFile(file);
            } else {
                tree = CommandLineLexer.LexCommandLine(args);
            }
            var context = Parser.Parse(tree);
            Injector.Inject(context);
            var generate = new Generate(context);
            Console.WriteLine(generate.BuildString());
        }
    }
}

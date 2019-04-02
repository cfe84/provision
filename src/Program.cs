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
            var file = args[0];
            var tree = YamlLexer.LoadResourcesFromFile(file);
            var context = Parser.Parse(tree);
            Injector.Inject(context);
            var generate = new Generate(context);
            Console.WriteLine(generate.BuildString());
        }
    }
}

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
            var context = new Context { DefaultLocation = "westus2"};
            context.Resources.Add(new ResourceGroup(context));
            context.Resources.Add(new StorageAccount(context, name: "output", accountPostfix: "out"));
            context.Resources.Add(new StorageAccount(context, "functions", "fun", new string[] { "toto", "titi" }));
            Injector.Inject(context);
            var generate = new Generate(context);
            Console.WriteLine(generate.BuildString());
        }
    }
}

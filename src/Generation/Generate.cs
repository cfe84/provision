using System;
using System.Collections.Generic;
using System.Linq;

namespace Provision
{
    class GenerationException : FunctionalException
    {
        public GenerationException(string message) : base(message) { }
    }
    class Generate
    {
        private Context context;
        public Generate(Context context)
        {
            this.context = context;
        }

        private IEnumerable<IResource> getOrderedResources() =>
            this.context.Resources
                .OrderBy(resource => resource.Order);
        private List<IResourceGenerator> getGeneratorList(IEnumerable<IResource> resourceList) =>
            resourceList
                .Select(resource => Resolver.GetResourceGenerator(resource))
                .ToList();

        public string CreateResourceListSummary(IEnumerable<IResource> resourceList) =>
            string.Join("\n", resourceList.Select(resource => $@"echo ""{resource.GetType().Name} ({resource.Name})"""));

        public string BuildString()
        {
            var sortedList = getOrderedResources();
            var generators = getGeneratorList(sortedList);
            var introduction = BaseDeclarations.Introduction(CreateResourceListSummary(sortedList));
            var declarations = string.Join("\n", generators
                .Select(generator => generator.GenerateResourceNameDeclaration())
                .Where(script => !String.IsNullOrWhiteSpace(script)));
            var cleanupScripts = string.Join("\n", generators
                .Select(generator => generator.GenerateCleanupScript())
                .Where(script => !String.IsNullOrWhiteSpace(script)));
            var provisioningScripts = string.Join("\n\n", generators
                .Select(generator => generator.GenerateProvisioningScript())
                .Where(script => !String.IsNullOrWhiteSpace(script)));
            var summaries = string.Join("\n", generators
                .Select(generator => generator.GenerateSummary())
                .Where(script => !String.IsNullOrWhiteSpace(script)));
            var envScripts = string.Join("\n", generators
                .Select(generator => generator.GenerateEnvScript())
                .Where(script => !String.IsNullOrWhiteSpace(script)));
            var envFileGenerator = BaseDeclarations.AssembleEnvFile(context);
            var envFileGeneratorAppend = BaseDeclarations.AssembleEnvFileAppending(declarations, envScripts);
            var result = BaseDeclarations.Header(this.context, envFileGenerator)
                + "\n"
                + introduction
                + "\n"
                + declarations
                + "\n"
                + provisioningScripts
                + "\n"
                + envFileGeneratorAppend
                + "\n"
                + BaseDeclarations.CleanupScript(cleanupScripts)
                + "\n"
                + summaries;
            return result.Replace("\r", "");
        }
    }
}
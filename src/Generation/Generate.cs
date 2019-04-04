using System.Collections.Generic;
using System.Linq;

namespace Provision {
    class Generate {
        private Context context;
        public Generate(Context context) {
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

        public string BuildString() {
            var sortedList = getOrderedResources();
            var generators = getGeneratorList(sortedList);
            var introduction = BaseDeclarations.Introduction(CreateResourceListSummary(sortedList));
            var declarations = string.Join("\n", generators
                .Select(generator => generator.GenerateResourceNameDeclaration()));
            var cleanupScripts = string.Join("\n", generators
                .Select(generator => generator.GenerateCleanupScript()));
            var provisioningScripts = string.Join("\n", generators
                .Select(generator => generator.GenerateProvisioningScript()));
            var summaries = string.Join("\n", generators
                .Select(generator => generator.GenerateSummary()));
            var result = BaseDeclarations.Header(this.context)
                + "\n"
                + introduction
                + "\n"
                + declarations
                + "\n"
                + provisioningScripts
                + "\n"
                + BaseDeclarations.CleanupScript(cleanupScripts)
                + "\n"
                + summaries;
            return result.Replace("\r", "");
        }
    }
}
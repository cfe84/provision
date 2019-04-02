using System.Collections.Generic;
using System.Linq;

namespace Provision {
    class Generate {
        private Context context;
        public Generate(Context context) {
            this.context = context;
        }

        private List<IResourceGenerator> getGeneratorList() => 
            this.context.Resources
                .OrderBy(resource => resource.Order)
                .Select(resource => Resolver.GetResourceGenerator(resource))
                .ToList();

        public string BuildString() {
            var generators = getGeneratorList();
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
                + declarations
                + BaseDeclarations.CleanupScript(cleanupScripts)
                + "\n"
                + provisioningScripts
                + "\n"
                + summaries;
            return result.Replace("\r", "");
        }
    }
}
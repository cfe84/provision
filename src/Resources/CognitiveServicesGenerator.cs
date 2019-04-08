using System.Collections.Generic;
using System.Linq;

namespace Provision {
    internal class CognitiveServicesGenerator : IResourceGenerator
    {
        private CognitiveServices cognitiveServices;
        public CognitiveServicesGenerator(CognitiveServices cognitiveServices) {
            this.cognitiveServices = cognitiveServices;
        }

        public string GenerateCleanupScript() => "";

        public string GenerateProvisioningScript() => $@"echo ""Creating cognitive services account ${cognitiveServices.CognitiveServicesAccountNameVariable}""
{cognitiveServices.CognitiveServicesEndpointVariable}=`az cognitiveservices account create --kind {cognitiveServices.Kind} --sku {cognitiveServices.Sku} -g ${cognitiveServices.ResourceGroup.ResourceGroupNameVariable} -n ${cognitiveServices.CognitiveServicesAccountNameVariable} -l ${cognitiveServices.Location} --yes --query ""endpoint"" -o tsv`
{cognitiveServices.CognitiveServicesKeyVariable}=`az cognitiveservices account keys list -g ${cognitiveServices.ResourceGroup.ResourceGroupNameVariable} --name ${cognitiveServices.CognitiveServicesAccountNameVariable} --query ""key1"" -o tsv`";
        public string GenerateResourceNameDeclaration() => $@"{cognitiveServices.CognitiveServicesAccountNameVariable}=""{cognitiveServices.CognitiveServicesName}""";

        public string GenerateSummary() => $@"echo ""      Cog. svc. account: ${cognitiveServices.CognitiveServicesAccountNameVariable}""
echo ""      Cog. svc endpoint: ${cognitiveServices.CognitiveServicesEndpointVariable}""
echo ""          Cog. svc. key: ${cognitiveServices.CognitiveServicesKeyVariable}""";
    }
}
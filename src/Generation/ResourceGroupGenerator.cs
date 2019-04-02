using System.Collections.Generic;

namespace Provision {
    internal class ResourceGroupGenerator : IResourceGenerator
    {
        public ResourceGroupGenerator(ResourceGroup resourceGroup) {
            this.resourceGroup = resourceGroup;
        }

        private ResourceGroup resourceGroup;
        
        public string GenerateResourceNameDeclaration() => $"{resourceGroup.ResourceGroupNameVariable}=\"{resourceGroup.ResourceGroupName}\"";

        public string GenerateCleanupScript() 
            => 
$@"echo 'Removing resource group ${resourceGroup.ResourceGroupNameVariable}'
az group delete --name ${resourceGroup.ResourceGroupNameVariable} --yes";

        public string GenerateProvisioningScript() 
            => 
$@"echo ""Creating resource group ${resourceGroup.ResourceGroupNameVariable}""
az group create --name ${resourceGroup.ResourceGroupNameVariable} --location ${resourceGroup.Location} --query ""properties.provisioningState"" -o tsv";

        public string GenerateSummary() 
            => $@"echo ""    Resource group name: ${resourceGroup.ResourceGroupNameVariable}""";

    }
}
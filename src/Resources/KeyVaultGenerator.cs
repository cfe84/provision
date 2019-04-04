using System.Collections.Generic;
using System.Linq;

namespace Provision {
    internal class KeyVaultGenerator : IResourceGenerator
    {
        private KeyVault keyVault;
        public KeyVaultGenerator(KeyVault keyVault) {
            this.keyVault = keyVault;
        }

        public string GenerateCleanupScript() => "";

        private string GenerateRoleAssignment() => 
            keyVault.ServicePrincipal == null ? "" : $@"
echo ""Granting permission""
az keyvault set-policy --name ${keyVault.ServicePrincipal.ApplicationIdVariable} --key-permissions create wrapKey unwrapKey get -g ${keyVault.ResourceGroup.ResourceGroupNameVariable} --spn ""${keyVault.ServicePrincipal.ApplicationIdentifierUriVariable}"" --query ""properties.provisioningState""";
        public string GenerateProvisioningScript() => $@"echo ""Creating Keyvault ${keyVault.KeyVaultVariableName}""
az keyvault create --name ${keyVault.KeyVaultVariableName} -g ${keyVault.ResourceGroup.ResourceGroupNameVariable} --location {keyVault.Location} --query ""properties.provisioningState"""
+ GenerateRoleAssignment();
        public string GenerateResourceNameDeclaration() => $@"{keyVault.KeyVaultVariableName}=""{keyVault.KeyVaultName}""";

        public string GenerateSummary() => $@"echo ""              Vault url: https:\/\/${keyVault.KeyVaultVariableName}.vault.azure.net\/""";


    }
}
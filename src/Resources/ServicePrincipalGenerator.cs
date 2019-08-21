using System.Collections.Generic;
using System.Linq;

namespace Provision
{
    internal class ServicePrincipalGenerator : IResourceGenerator
    {
        private ServicePrincipal servicePrincipal;
        public ServicePrincipalGenerator(ServicePrincipal servicePrincipal)
        {
            this.servicePrincipal = servicePrincipal;
        }

        public string GenerateCleanupScript() => $@"echo 'Removing service principal ${servicePrincipal.ApplicationIdentifierUriVariable}'
az ad sp delete --id ${servicePrincipal.ApplicationIdVariable}";

        public string GenerateEnvScript() => $@"IFS=$'\t' read -r {servicePrincipal.ApplicationIdVariable} {servicePrincipal.PasswordVariable} <<< `az ad sp show --id ""${servicePrincipal.ApplicationIdentifierUriVariable}"" --query ""{{appId: appId, password: password}}"" -o tsv`";

        public string GenerateProvisioningScript() => $@"echo ""Creating Service Principal ${servicePrincipal.ApplicationIdentifierUriVariable}""
IFS=$'\t' read -r {servicePrincipal.ApplicationIdVariable} {servicePrincipal.PasswordVariable} <<< `az ad sp create-for-rbac --name ""${servicePrincipal.ApplicationIdentifierUriVariable}"" --query ""{{appId: appId, password: password}}"" -o tsv`";
        public string GenerateResourceNameDeclaration() => $@"{servicePrincipal.ApplicationIdentifierUriVariable}=""{servicePrincipal.ApplicationName}""";

        public string GenerateSummary() => $@"echo ""                 App id: ${servicePrincipal.ApplicationIdVariable}""
echo ""                App URI: ${servicePrincipal.ApplicationIdentifierUriVariable}""
echo ""           App password: ${servicePrincipal.PasswordVariable}""";


    }
}
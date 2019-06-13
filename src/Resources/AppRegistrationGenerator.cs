using System.Collections.Generic;
using System.Linq;

namespace Provision {
    internal class AppRegistrationGenerator : IResourceGenerator
    {
        private AppRegistration appRegistration;
        public AppRegistrationGenerator(AppRegistration appRegistration) {
            this.appRegistration = appRegistration;
            if (appRegistration.ReplyUrl == null) {
                throw new GenerationException("App registration doesn't have a reply url. Either specify one or add a function app or webapp");
            }
        }

        public string GenerateCleanupScript() => $@"echo 'Removing app registration ${appRegistration.ApplicationIdentifierUriVariable}'
az ad app delete --id ${appRegistration.ApplicationIdentifierUriVariable}";

        public string GenerateGroupMembershipClaims() => 
            appRegistration.GroupMembershipClaims != null 
            ? $@"
az ad app update --id ${appRegistration.ApplicationIdentifierUriVariable} --set groupMembershipClaims={appRegistration.GroupMembershipClaims}"
            : "";

        public string GenerateProvisioningScript() => $@"echo ""Creating App Registration ${appRegistration.ApplicationIdentifierUriVariable}""
echo ""[{{
    'resourceAppId': '00000002-0000-0000-c000-000000000000',
    'resourceAccess': [ {{
        'id': '311a71cc-e848-46a1-bdf8-97ff7156d8e6',
        'type': 'Scope'
        }} ]
    }}]"" > app-registration-manifest.tmp.json
{appRegistration.ApplicationIdVariable}=`az ad app create --identifier-uris ${appRegistration.ApplicationIdentifierUriVariable} --available-to-other-tenants {appRegistration.AvailableToOtherTenants} --reply-urls {appRegistration.ReplyUrl} --display-name ${appRegistration.ApplicationIdentifierUriVariable} --password ""${appRegistration.PasswordVariable}"" --required-resource-access app-registration-manifest.tmp.json --query ""appId"" -o tsv`
rm app-registration-manifest.tmp.json" + GenerateGroupMembershipClaims();

        public string GenerateResourceNameDeclaration() => 
            $@"{appRegistration.ApplicationIdentifierUriVariable}=""{appRegistration.IdentifierUri}""
{appRegistration.PasswordVariable}=""@$RANDOMBASE16CHAR^_132001""";

        public string GenerateSummary() => $@"echo ""                 App id: ${appRegistration.ApplicationIdVariable}""
echo ""                App URI: ${appRegistration.ApplicationIdentifierUriVariable}""
echo ""           App password: ${appRegistration.PasswordVariable}""";


    }
}
using System.Collections.Generic;
using System.Linq;

namespace Provision {
    internal class AppServicePlanGenerator : IResourceGenerator
    {
        private AppServicePlan appServicePlan;
        public AppServicePlanGenerator(AppServicePlan appServicePlan) {
            this.appServicePlan = appServicePlan;
        }

        public string GenerateCleanupScript() => "";
        public string GenerateProvisioningScript() => $@"echo ""Creating app service plan ${appServicePlan.AppServicePlanAccountVariableName}""
az webapp deployment user set --user-name ""$DEPLOYMENTUSERNAME"" --password ""$DEPLOYMENTPASSWORD"" > /dev/null
az appservice plan create -g ${appServicePlan.ResourceGroup.ResourceGroupNameVariable} -n ${appServicePlan.AppServicePlanAccountVariableName} --sku {appServicePlan.SKU} --location {appServicePlan.Location} --query ""provisioningState"" -o tsv";
        public string GenerateResourceNameDeclaration() => $@"{appServicePlan.AppServicePlanAccountVariableName}=""$NAME-`random 5`""
DEPLOYMENTUSERNAME=""{appServicePlan.DeploymentUserName}""
DEPLOYMENTPASSWORD=""{appServicePlan.DeploymentPassword}""";
        public string GenerateSummary() => $@"echo ""       App service plan: ${appServicePlan.AppServicePlanAccountVariableName}""
echo ""        Deployment user: $DEPLOYMENTUSERNAME""
echo ""    Deployment password: $DEPLOYMENTPASSWORD""";
    }
}
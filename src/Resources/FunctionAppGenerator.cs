using System;
using System.Collections.Generic;
using System.Linq;

namespace Provision {
    internal class FunctionAppGenerator : IResourceGenerator
    {
        private FunctionApp functionApp;
        public FunctionAppGenerator(FunctionApp functionApp) {
            this.functionApp = functionApp;
        }

        public string GenerateCleanupScript() => "";
        private string GenerateSettings() => 
            functionApp.Settings != null ? $@"az functionapp config appsettings set --name ${functionApp.FunctionAppVariableName} -g ${functionApp.ResourceGroup.ResourceGroupNameVariable} --settings {string.Join(" ", functionApp.Settings)} > /dev/null
"
            : "";
        private string GenerateDeploy() =>
            functionApp.Deploy.Equals("true", StringComparison.InvariantCultureIgnoreCase) ? 
            $@"echo ""Deploying function app ${functionApp.FunctionAppVariableName}""
echo ""Deploying functionapp""
func azure functionapp publish ${functionApp.FunctionAppVariableName}
""" : "";

        private string getPlanOptions() =>
        functionApp.AppServicePlan == null 
            ? $"--consumption-plan-location ${functionApp.Location}"
            : $"--plan ${functionApp.AppServicePlan.AppServicePlanAccountVariableName}";

        public string GenerateProvisioningScript() => $@"echo ""Creating functionapp ${functionApp.FunctionAppVariableName}""
az functionapp create -g ${functionApp.ResourceGroup.ResourceGroupNameVariable} {getPlanOptions()} --name ${functionApp.FunctionAppVariableName} --storage-account ${functionApp.StorageAccount.StorageAccountVariableName} --query ""state"" -o tsv

" 
+ GenerateSettings()
+ GenerateDeploy();

        public string GenerateResourceNameDeclaration() => $@"{functionApp.FunctionAppVariableName}=""{functionApp.FunctionAppName}""";
        public string GenerateSummary() => $@"echo ""          Function Name: ${functionApp.FunctionAppVariableName}""
echo ""           Function URL: https://${functionApp.HostNameVariable}""
";


    }
}
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
            functionApp.Settings != null ? $@"
az functionapp config appsettings set --name ${functionApp.FunctionAppVariableName} -g ${functionApp.ResourceGroup.ResourceGroupNameVariable} --settings {string.Join(" ", functionApp.Settings)} > /dev/null"
            : "";
        private string GenerateDeploy() =>
            functionApp.Deploy.Equals("true", StringComparison.InvariantCultureIgnoreCase) ? 
            $@"
echo ""Deploying function app ${functionApp.FunctionAppVariableName}""
func azure functionapp publish ${functionApp.FunctionAppVariableName}" : "";

        private string getPlanOptions() =>
        functionApp.AppServicePlan == null 
            ? $"--consumption-plan-location ${functionApp.Location}"
            : $"--plan ${functionApp.AppServicePlan.AppServicePlanAccountVariableName}";

        private string GenerateEasyAuth() =>
            functionApp.AppRegistration != null ?
            $@"
echo ""
Configuring easy auth for functionapp ${functionApp.FunctionAppVariableName}""
az webapp auth update --ids ${functionApp.ResourceGroup.ResourceGroupResourceIdVariable}/providers/Microsoft.Web/sites/${functionApp.FunctionAppVariableName} --action LoginWithAzureActiveDirectory --enabled true --aad-client-id ${functionApp.AppRegistration.ApplicationIdVariable} --aad-client-secret ""${functionApp.AppRegistration.PasswordVariable}"" --aad-token-issuer-url https://login.microsoftonline.com/{functionApp.TenantId}/  > /dev/null"
            : "";

        public string GenerateProvisioningScript() => $@"echo ""Creating functionapp ${functionApp.FunctionAppVariableName}""
az functionapp create -g ${functionApp.ResourceGroup.ResourceGroupNameVariable} {getPlanOptions()} --name ${functionApp.FunctionAppVariableName} --storage-account ${functionApp.StorageAccount.StorageAccountVariableName} --query ""state"" -o tsv" 
+ GenerateSettings()
+ GenerateEasyAuth()
+ GenerateDeploy();

        public string GenerateResourceNameDeclaration() => $@"{functionApp.FunctionAppVariableName}=""{functionApp.FunctionAppName}""
{functionApp.HostNameVariable}=""https://${functionApp.FunctionAppVariableName}.azurewebsites.net""
";
        public string GenerateSummary() => $@"echo ""          Function Name: ${functionApp.FunctionAppVariableName}""
echo ""           Function URL: ${functionApp.HostNameVariable}""
";


    }
}
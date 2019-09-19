using System;
using System.Collections.Generic;
using System.Linq;

namespace Provision
{
    internal class FunctionAppGenerator : IResourceGenerator
    {
        private FunctionApp functionApp;
        public FunctionAppGenerator(FunctionApp functionApp)
        {
            this.functionApp = functionApp;
        }

        public string GenerateCleanupScript() =>
            functionApp.IdentityScope != null ? $@"echo 'Removing function app identity ${functionApp.IdentityRolePrincipalIdVariable}'
# az ad app delete --id ${functionApp.IdentityRolePrincipalIdVariable}"
            : "";

        private string GenerateSettings() =>
            functionApp.Settings != null ? $@"
az functionapp config appsettings set --name ${functionApp.FunctionAppVariableName} -g ${functionApp.ResourceGroup.ResourceGroupNameVariable} --settings {string.Join(" ", functionApp.Settings)} > /dev/null"
            : "";

        private string CwdForDeployment() =>
            functionApp.DeployPath != null ? $@"
cd ""{functionApp.DeployPath}"""
: "";

        private string PwdForDeployment() =>
            functionApp.DeployPath != null ? $@"
cd $PWD"
: "";

        private string GenerateDeploy() =>
            functionApp.Deploy.Equals("true", StringComparison.InvariantCultureIgnoreCase) ?
            $@"
echo ""Deploying function app ${functionApp.FunctionAppVariableName}""" + CwdForDeployment() + $@"
func azure functionapp publish ${functionApp.FunctionAppVariableName}" + PwdForDeployment()
: "";

        private string getPlanOptions() =>
        functionApp.AppServicePlan == null
            ? $"--consumption-plan-location ${functionApp.Location}"
            : $"--plan ${functionApp.AppServicePlan.AppServicePlanAccountVariableName}";

        private string GenerateEasyAuth() =>
            functionApp.AppRegistration != null ?
            $@"
echo ""Configuring easy auth for functionapp ${functionApp.FunctionAppVariableName}""
az webapp auth update --ids ${functionApp.ResourceGroup.ResourceGroupResourceIdVariable}/providers/Microsoft.Web/sites/${functionApp.FunctionAppVariableName} --action LoginWithAzureActiveDirectory --enabled true --aad-client-id ${functionApp.AppRegistration.ApplicationIdVariable} --aad-client-secret ""${functionApp.AppRegistration.PasswordVariable}"" --aad-token-issuer-url https://login.microsoftonline.com/{functionApp.TenantId}/  > /dev/null"
            : "";

        private string GenerateIdentity() =>
            functionApp.IdentityScope != null ?
            $@"
echo ""Configuring identity for functionapp ${functionApp.FunctionAppVariableName} with {functionApp.IdentityRole} access to scope {functionApp.IdentityScope}.""
{functionApp.IdentityRolePrincipalIdVariable}=`az functionapp identity assign --name ${functionApp.FunctionAppVariableName} --resource-group ${functionApp.ResourceGroup.ResourceGroupNameVariable} --role {functionApp.IdentityRole} --scope ""{functionApp.IdentityScope}"" --query principalId -o tsv`"
            : "";

        public string GenerateProvisioningScript() => $@"echo ""Creating functionapp ${functionApp.FunctionAppVariableName}""
az functionapp create -g ${functionApp.ResourceGroup.ResourceGroupNameVariable} {getPlanOptions()} --name ${functionApp.FunctionAppVariableName} --storage-account ${functionApp.StorageAccount.StorageAccountVariableName} --query ""state"" -o tsv"
+ GenerateSettings()
+ GenerateEasyAuth()
+ GenerateIdentity()
+ GenerateDeploy();

        public string GenerateResourceNameDeclaration() => $@"{functionApp.FunctionAppVariableName}=""{functionApp.FunctionAppName}""
{functionApp.HostNameVariable}=""https://${functionApp.FunctionAppVariableName}.azurewebsites.net""
";

        private string FunctionAppSummaryPrincipal() =>
            functionApp.IdentityScope != null ? $@"
echo ""  Function id principal: ${functionApp.IdentityRolePrincipalIdVariable}"""
            : "";

        public string GenerateSummary() => $@"echo ""          Function Name: ${functionApp.FunctionAppVariableName}""
echo ""           Function URL: ${functionApp.HostNameVariable}"""
            + FunctionAppSummaryPrincipal();

        public string GenerateEnvScript() => "";
    }
}
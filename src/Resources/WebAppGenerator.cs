using System;
using System.Collections.Generic;
using System.Linq;

namespace Provision {
    internal class WebAppGenerator : IResourceGenerator
    {
        private WebApp webApp;
        public WebAppGenerator(WebApp webApp) {
            this.webApp = webApp;
        }

        public string GenerateCleanupScript() => "";
        private string GenerateSettings() => 
            webApp.Settings != null ? $@"az webapp config appsettings set --name ${webApp.WebAppVariableName} -g ${webApp.ResourceGroup.ResourceGroupNameVariable} --settings {string.Join(" ", webApp.Settings)} > /dev/null
"
            : "";
        private string GenerateDeploy() =>
            webApp.Deploy.Equals("true", StringComparison.InvariantCultureIgnoreCase) ? 
            $@"echo ""Deploying webapp""
git remote add azure ""${webApp.GitUrlVariable}""
git push azure master
""" : "";

        public string GenerateProvisioningScript() => $@"echo ""Creating webapp ${webApp.WebAppVariableName}""
{webApp.HostNameVariable}=`az webapp create -g ${webApp.ResourceGroup.ResourceGroupNameVariable} -n ${webApp.WebAppVariableName} --plan ${webApp.AppServicePlan.AppServicePlanAccountVariableName} --deployment-local-git --query ""defaultHostName"" -o tsv`
{webApp.GitUrlVariable}=""https://$DEPLOYMENTUSERNAME:$DEPLOYMENTPASSWORD@${webApp.WebAppVariableName}.scm.azurewebsites.net/${webApp.WebAppVariableName}.git""
" 
+ GenerateSettings()
+ GenerateDeploy();

        public string GenerateResourceNameDeclaration() => $@"{webApp.WebAppVariableName}=""{webApp.WebAppName}""";
        public string GenerateSummary() => $@"echo ""            Webapp Name: ${webApp.WebAppVariableName}""
echo ""             Webapp URL: https://${webApp.HostNameVariable}""
echo ""         Webapp Git URL: ${webApp.GitUrlVariable}""
";


    }
}
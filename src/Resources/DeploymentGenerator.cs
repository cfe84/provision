namespace Provision
{
    internal class DeploymentGenerator : IResourceGenerator
    {
        private Deployment deployment;
        public DeploymentGenerator(Deployment deployment)
        {
            this.deployment = deployment;
        }
        public string GenerateCleanupScript() => "";

        private string generateUri() => deployment.Uri != null
            ? $" --template-uri {deployment.Uri} "
            : "";

        private string generateFile() => deployment.File != null
            ? $" --template-file {deployment.File} "
            : "";

        private string generateParameters() => deployment.Parameters.Length > 0
            ? $" --parameters {string.Join(" ", deployment.Parameters)} "
            : "";

        public string GenerateProvisioningScript() => $@"echo ""Deploying ARM template {deployment.Name}""
az group deployment create -g ""${deployment.ResourceGroup.ResourceGroupNameVariable}"" {generateUri()} {generateFile()} --name ""{deployment.Name}"" {generateParameters()} --query 'properties.provisioningState' -o tsv";

        public string GenerateResourceNameDeclaration() => "";

        public string GenerateSummary() => "";

        public string GenerateEnvScript() => "";
    }
}
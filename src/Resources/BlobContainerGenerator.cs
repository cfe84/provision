namespace Provision
{
    class BlobContainerGenerator : IResourceGenerator
    {
        private readonly BlobContainer container;

        public BlobContainerGenerator(BlobContainer container)
        {
            this.container = container;
        }

        public string GenerateCleanupScript() => "";

        public string GenerateEnvScript() => "";

        public string GenerateProvisioningScript() => $@"echo ""Creating container ${container.StorageAccount.Name}/${container.Variable}""
echo -n ""Created: ""
az storage container create --name ""${container.Variable}"" --account-name ${container.StorageAccount.StorageAccountVariableName} --query ""created"" -o tsv""";

        public string GenerateResourceNameDeclaration() => $@"{container.Variable}=""{container.ContainerName}""";

        public string GenerateSummary() => $@"echo ""         Container name: ${container.Variable}""";

    }
}
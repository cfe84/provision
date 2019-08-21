using System.Collections.Generic;
using System.Linq;

namespace Provision
{
    internal class StorageAccountGenerator : IResourceGenerator
    {
        private StorageAccount storageAccount;
        public StorageAccountGenerator(StorageAccount storageAccount)
        {
            this.storageAccount = storageAccount;
        }

        public string GenerateCleanupScript() => "";

        private string GenerateContainers() =>
            storageAccount.Containers.Length > 0 ? "\n" + string.Join("\n",
                storageAccount.Containers.Select((System.Func<string, string>)(
                    containerName => $@"echo ""Creating container ${storageAccount.StorageAccountVariableName}.{containerName}""
az storage container create --name ""{containerName}"" --account-name ${storageAccount.StorageAccountVariableName} --query ""created"" -o tsv")))
                : "";

        private string GenerateQueues() =>
            storageAccount.Queues.Length > 0 ? "\n" + string.Join("\n",
                storageAccount.Queues.Select((System.Func<string, string>)(
                    queueName => $@"echo ""Creating queue ${storageAccount.StorageAccountVariableName}.{queueName}""
az storage queue create --name ""{queueName}"" --account-name ${storageAccount.StorageAccountVariableName} --query ""created"" -o tsv")))
                : "";

        private string GenerateTables() =>
            storageAccount.Tables.Length > 0 ? "\n" + string.Join("\n",
                storageAccount.Tables.Select((System.Func<string, string>)(
                    tableName => $@"echo ""Creating table ${storageAccount.StorageAccountVariableName}.{tableName}""
az storage table create --name ""{tableName}"" --account-name ${storageAccount.StorageAccountVariableName} --query ""created"" -o tsv")))
            : "";

        private string GenerateCors() =>
            storageAccount.CorsMethods != null ? $@"
echo ""Setting up CORS on storage account ${storageAccount.StorageAccountVariableName}""
az storage cors add --account-name ${storageAccount.StorageAccountVariableName} --origins '*' --methods {storageAccount.CorsMethods} --allowed-headers '*' --exposed-headers '*' --max-age 200 --services b"
            : "";

        public string GenerateProvisioningScript() => $@"echo ""Creating storage account ${storageAccount.StorageAccountVariableName}""
az storage account create --name ${storageAccount.StorageAccountVariableName} --kind StorageV2 --sku {storageAccount.SKU} --location {storageAccount.Location} -g ${storageAccount.ResourceGroup.ResourceGroupNameVariable} --https-only true --query ""provisioningState"" -o tsv
{storageAccount.ConnectionStringVariableName}=`az storage account show-connection-string -g ${storageAccount.ResourceGroup.ResourceGroupNameVariable} -n ${storageAccount.StorageAccountVariableName} --query connectionString -o tsv`"
            + GenerateContainers() + GenerateQueues() + GenerateTables() + GenerateCors();
        public string GenerateResourceNameDeclaration() => $@"{storageAccount.StorageAccountVariableName}=""`echo ""$STORAGEBASENAME"" | sed -e 's/-//g' | sed -E 's/^(.*)$/\L\1/g' | head -c 20`{storageAccount.AccountPostfix}""
{storageAccount.StorageAccountResourceIdVariableName}=""${storageAccount.ResourceGroup.ResourceGroupResourceIdVariable}/providers/Microsoft.Storage/storageAccounts/${storageAccount.StorageAccountVariableName}""";

        public string GenerateSummary() => $@"echo ""  Storage account ({storageAccount.AccountPostfix}): ${storageAccount.StorageAccountVariableName}""
echo ""      Storage key ({storageAccount.AccountPostfix}): ${storageAccount.ConnectionStringVariableName}""";

        public string GenerateEnvScript() => $@"{storageAccount.ConnectionStringVariableName}=`az storage account show-connection-string -g ${storageAccount.ResourceGroup.ResourceGroupNameVariable} -n ${storageAccount.StorageAccountVariableName} --query connectionString -o tsv`""";
    }
}
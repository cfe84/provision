using System.Collections.Generic;
using System.Linq;

namespace Provision {
    internal class StorageAccountGenerator : IResourceGenerator
    {
        private StorageAccount storageAccount;
        public StorageAccountGenerator(StorageAccount storageAccount) {
            this.storageAccount = storageAccount;
        }

        public string GenerateCleanupScript() => "";

        private string GenerateContainers() => 
            "\n" + string.Join("\n", 
                storageAccount.Containers.Select((System.Func<string, string>)(
                    containerName => $@"echo ""Creating container ${storageAccount.StorageAccountVariableName}.{containerName}""
az storage container create --name ""{containerName}"" --account-name ${storageAccount.StorageAccountVariableName} --query ""created"" -o tsv
")));

        public string GenerateProvisioningScript() => $@"echo ""Creating storage account ${storageAccount.StorageAccountVariableName}""
az storage account create --name ${storageAccount.StorageAccountVariableName} --kind StorageV2 --location {storageAccount.Location} -g ${storageAccount.ResourceGroup.ResourceGroupNameVariable} --https-only true --query ""provisioningState"" -o tsv
{storageAccount.ConnectionStringVariableName}=`az storage account show-connection-string -g ${storageAccount.ResourceGroup.ResourceGroupNameVariable} -n ${storageAccount.StorageAccountVariableName} --query connectionString -o tsv`"
            + GenerateContainers();
        public string GenerateResourceNameDeclaration() => $@"{storageAccount.StorageAccountVariableName}=""`echo ""$STORAGEBASENAME"" | sed -e 's/-//g' | sed -E 's/^(.*)$/\L\1/g' | head -c 20`{storageAccount.AccountPostfix}""";

        public string GenerateSummary() => $@"echo ""  Storage account ({storageAccount.AccountPostfix}): ${storageAccount.StorageAccountVariableName}""
echo ""      Storage key ({storageAccount.AccountPostfix}): ${storageAccount.ConnectionStringVariableName}""";


    }
}
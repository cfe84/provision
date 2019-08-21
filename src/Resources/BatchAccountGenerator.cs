using System.Collections.Generic;
using System.Linq;

namespace Provision
{
    internal class BatchAccountGenerator : IResourceGenerator
    {
        private BatchAccount batchAccount;
        public BatchAccountGenerator(BatchAccount appServicePlan)
        {
            this.batchAccount = appServicePlan;
        }

        public string GenerateCleanupScript() => "";

        public string GenerateProvisioningScript() => $@"echo ""Creating batch account ${batchAccount.BatchAccountNameVariable}""
BATCH_ACCOUNT_ENDPOINT=`az batch account create -g ${batchAccount.ResourceGroup.ResourceGroupNameVariable} --name ${batchAccount.BatchAccountNameVariable} --location {batchAccount.Location} --storage-account ${batchAccount.StorageAccount.StorageAccountVariableName} --query accountEndpoint -o tsv`
BATCH_ACCOUNT_KEY=`az batch account keys list -g ${batchAccount.ResourceGroup.ResourceGroupNameVariable} --name ${batchAccount.BatchAccountNameVariable} --query primary -o tsv`
az batch account login -g ${batchAccount.ResourceGroup.ResourceGroupNameVariable} --name ${batchAccount.BatchAccountNameVariable} --shared-key-auth
echo ""{{
    \""id\"": \""${batchAccount.BatchPoolNameVariable}\"",
    \""vmSize\"": \""{batchAccount.VmSize}\"",
    \""virtualMachineConfiguration\"": {{
        \""imageReference\"": {{
            \""publisher\"": \""{batchAccount.ImagePublisher}\"",
            \""offer\"": \""{batchAccount.ImageOffer}\"",
            \""sku\"": \""{batchAccount.ImageSku}\""
        }},
        \""nodeAgentSKUId\"": \""{batchAccount.NodeAgentSku}\""
    }},
    \""resizeTimeout\"": \""PT15M\"",
    \""targetDedicatedNodes\"": {batchAccount.TargetDedicatedNodes},
    \""targetLowPriorityNodes\"": {batchAccount.TargetLowPriorityNodes},
    \""maxTasksPerNode\"": 3,
    \""taskSchedulingPolicy\"": {{
        \""nodeFillType\"": \""spread\""
    }},
    \""enableAutoScale\"": false,
    \""enableInterNodeCommunication\"": true,
    \""metadata\"": [
    ],
    \""startTask\"": {{
        \""commandLine\"": \""{batchAccount.StartTask}\"",
        \""maxTaskRetryCount\"": 0,
        \""waitForSuccess\"": true,
        \""userIdentity\"": {{
            \""autoUser\"": {{
                \""elevationLevel\"": \""admin\"",
                \""scope\"": \""pool\""
            }}
        }}
    }}
}}"" > pool.json
az batch pool create --json-file pool.json
rm pool.json" + CreateApplication();

        private string ZipFileName() => batchAccount.ApplicationZip ?? "package.zip";

        private string CreateZip() => batchAccount.ApplicationFolder != null ?
            $@"
cd {batchAccount.ApplicationFolder} && zip -ll package.zip * && cd .. && mv {batchAccount.ApplicationFolder}/package.zip {ZipFileName()}" : "";

        private string CreateApplication() => HasApplication() ?
            $@"
echo ""Creating application ${batchAccount.BatchApplicationNameVariable}""
az batch application create -g ${batchAccount.ResourceGroup.ResourceGroupNameVariable} -n ${batchAccount.BatchAccountNameVariable} --application-name ${batchAccount.BatchApplicationNameVariable} --query ""name"" -o tsv" +
CreateZip() + $@"
az batch application package create -g ${batchAccount.ResourceGroup.ResourceGroupNameVariable} -n ${batchAccount.BatchAccountNameVariable} --application-name ${batchAccount.BatchApplicationNameVariable} --version-name ${batchAccount.BatchApplicationVersionVariable} --package-file {ZipFileName()} --query state -o tsv" : "";
        private string AddApplicationDeclaration() => HasApplication() ?
            $@"
{batchAccount.BatchApplicationNameVariable}=""{batchAccount.ApplicationName}""
{batchAccount.BatchApplicationVersionVariable}=""{batchAccount.ApplicationVersion}""" : "";

        private bool HasApplication() =>
            batchAccount.ApplicationZip != null || batchAccount.ApplicationFolder != null;

        public string GenerateResourceNameDeclaration() => $@"{batchAccount.BatchAccountNameVariable}=""{batchAccount.BatchAccountName}""
{batchAccount.BatchPoolNameVariable}=""{batchAccount.BatchPoolName}""" + AddApplicationDeclaration();
        public string GenerateSummary() => $@"echo ""          Batch account: ${batchAccount.BatchAccountNameVariable}""
echo ""      Batch account key: ${batchAccount.BatchAccountKeyVariable}""";

        public string GenerateEnvScript() => $@"BATCH_ACCOUNT_ENDPOINT=`az batch account show -g ${batchAccount.ResourceGroup.ResourceGroupNameVariable} --name ${batchAccount.BatchAccountNameVariable} --query accountEndpoint -o tsv
BATCH_ACCOUNT_KEY=`az batch account keys list -g ${batchAccount.ResourceGroup.ResourceGroupNameVariable} --name ${batchAccount.BatchAccountNameVariable} --query primary -o tsv`";
    }
}
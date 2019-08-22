using System.Collections.Generic;
using System.Linq;

namespace Provision
{
    internal class EventHubGenerator : IResourceGenerator
    {
        private EventHub eventHub;
        public EventHubGenerator(EventHub eventHub)
        {
            this.eventHub = eventHub;
        }

        public string GenerateCleanupScript() => "";

        public string GenerateProvisioningScript()
        {
            var archive = eventHub.ArchiveStorageAccount != null ? $"--storage-account \"${eventHub.ArchiveStorageAccount.StorageAccountResourceIdVariableName}\" --blob-container \"{eventHub.ArchiveBlobContainer}\"" : "";
            var partitionCount = string.IsNullOrEmpty(eventHub.PartitionCount) ? "" : $"--partition-count {eventHub.PartitionCount}";
            return $@"echo ""Creating eventhub ${eventHub.EventHubNamespace.EventHubNamespaceVariable}.${eventHub.EventHubVariable}""
az eventhubs eventhub create --name ""${eventHub.EventHubVariable}"" -g ""${eventHub.EventHubNamespace.ResourceGroup.ResourceGroupNameVariable}"" --namespace-name ""${eventHub.EventHubNamespace.EventHubNamespaceVariable}"" --message-retention {eventHub.MessageRetention} {archive} {partitionCount} --query provisionState -o tsv
az eventhubs eventhub authorization-rule create --eventhub-name ""${eventHub.EventHubVariable}"" -g ""${eventHub.EventHubNamespace.ResourceGroup.ResourceGroupNameVariable}"" --namespace-name ""${eventHub.EventHubNamespace.EventHubNamespaceVariable}"" --name admin --rights Manage Send Listen --query name --name ""${eventHub.KeyNameVariable}"" -o tsv
IFS=$'\t' read -r {eventHub.ConnectionStringVariable} {eventHub.KeyVariable} <<< `az eventhubs eventhub authorization-rule keys list --eventhub-name ""${eventHub.EventHubVariable}"" -g ""${eventHub.EventHubNamespace.ResourceGroup.ResourceGroupNameVariable}"" --namespace-name ""${eventHub.EventHubNamespace.EventHubNamespaceVariable}"" --name ""${eventHub.KeyNameVariable}"" -o tsv --query ""{{connectionString: primaryConnectionString, primaryKey: primaryKey}}""`
 ";
        }

        public string GenerateResourceNameDeclaration() => $@"{eventHub.EventHubVariable}=""{eventHub.EventHubName}""
{eventHub.KeyNameVariable}=""{eventHub.AuthorizationRuleName}""";

        public string GenerateSummary() => $@"echo ""  EventHub ({eventHub.Name}): ${eventHub.EventHubVariable}""";

        public string GenerateEnvScript() => $@"";
    }
}
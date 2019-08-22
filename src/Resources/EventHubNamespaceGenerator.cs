using System.Collections.Generic;
using System.Linq;

namespace Provision
{
    internal class EventHubNamespaceGenerator : IResourceGenerator
    {
        private EventHubNamespace eventHubNamespace;
        public EventHubNamespaceGenerator(EventHubNamespace eventHubNamespace)
        {
            this.eventHubNamespace = eventHubNamespace;
        }

        public string GenerateCleanupScript() => "";

        public string GenerateProvisioningScript()
        {
            var maxThroughput = eventHubNamespace.EnableAutoInflate == "true" ? "--maximum-throughput-units {eventHub.ThroughputUnits}" : "";
            return $@"echo ""Creating eventhub namespace ${eventHubNamespace.EventHubNamespaceVariable}""
az eventhubs namespace create --name ${eventHubNamespace.EventHubNamespaceVariable} --sku {eventHubNamespace.SKU} --location {eventHubNamespace.Location} --capacity {eventHubNamespace.Capacity} -g ${eventHubNamespace.ResourceGroup.ResourceGroupNameVariable} --enable-auto-inflate {eventHubNamespace.EnableAutoInflate} {maxThroughput} --enable-kafka {eventHubNamespace.EnableKafka} --query ""provisioningState"" -o tsv";
        }

        public string GenerateResourceNameDeclaration() => $@"{eventHubNamespace.EventHubNamespaceVariable}=""{eventHubNamespace.EventHubNamespaceName}""";

        public string GenerateSummary() => $@"echo ""  EventHub namespace ({eventHubNamespace.Name}): ${eventHubNamespace.EventHubNamespaceVariable}""";

        public string GenerateEnvScript() => $@"";
    }
}
using System.Collections.Generic;
using System.Linq;

namespace Provision
{
    internal class ServiceBusGenerator : IResourceGenerator
    {
        private ServiceBus serviceBus;
        public ServiceBusGenerator(ServiceBus serviceBus)
        {
            this.serviceBus = serviceBus;
        }

        public string GenerateCleanupScript() => "";

        private string GenerateTopics() =>
            serviceBus.Topics.Length > 0 ? "\n" + string.Join("\n",
                serviceBus.Topics.Select((System.Func<string, string>)(
                    topicName => $@"echo ""Creating topic ${serviceBus.ServiceBusVariable}.{topicName}""
az servicebus topic create -g ""${serviceBus.ResourceGroup.ResourceGroupNameVariable}"" --name ""{topicName}"" --namespace-name ${serviceBus.ServiceBusVariable} --query ""created"" -o tsv")))
                : "";

        private string GenerateQueues() =>
            serviceBus.Queues.Length > 0 ? "\n" + string.Join("\n",
                serviceBus.Queues.Select((System.Func<string, string>)(
                    queueName => $@"echo ""Creating queue ${serviceBus.ServiceBusVariable}.{queueName}""
az servicebus queue create -g ""${serviceBus.ResourceGroup.ResourceGroupNameVariable}"" --name ""{queueName}"" --namespace-name ${serviceBus.ServiceBusVariable} --query ""created"" -o tsv")))
                : "";

        private string GenerateSubscriptions() =>
            serviceBus.Subscriptions.Length > 0 ? "\n" + string.Join("\n",
                serviceBus.Subscriptions.Select((System.Func<string, string>)(
                    subscription => $@"echo ""Creating subscription ${serviceBus.ServiceBusVariable}.{subscription}""
az servicebus topic subscription create -g ""${serviceBus.ResourceGroup.ResourceGroupNameVariable}"" --name ""{subscription}"" --namespace-name ${serviceBus.ServiceBusVariable} --topic-name {serviceBus.Topics[0]} --query ""created"" -o tsv")))
            : "";

        public string GenerateProvisioningScript()
        {
            var capacity = string.IsNullOrEmpty(serviceBus.Capacity) ? "" : "--capacity " + serviceBus.Capacity;
            return $@"echo ""Creating service bus namespace ${serviceBus.ServiceBusVariable}""
az servicebus namespace create --name ${serviceBus.ServiceBusVariable} --sku {serviceBus.SKU} {capacity} --location {serviceBus.Location} -g ${serviceBus.ResourceGroup.ResourceGroupNameVariable} --query ""provisioningState"" -o tsv
{serviceBus.KeyNameVariable}=""RootManageSharedAccessKey""
IFS=$'\t' read -r {serviceBus.ConnectionStringVariable} {serviceBus.KeyVariable} <<< `az servicebus namespace authorization-rule keys list -g ""${serviceBus.ResourceGroup.ResourceGroupNameVariable}"" --namespace-name ""${serviceBus.ServiceBusVariable}"" -n ""${serviceBus.KeyNameVariable}"" -o tsv --query ""{{connectionString: primaryConnectionString, primaryKey: primaryKey}}""`"
            + GenerateTopics() + GenerateQueues() + GenerateSubscriptions();
        }

        public string GenerateResourceNameDeclaration() => $@"{serviceBus.ServiceBusVariable}=""{serviceBus.ServiceBusName}""";

        public string GenerateSummary() => $@"echo ""  Service bus   ({serviceBus.Name}): ${serviceBus.ServiceBusVariable}""
echo ""      Key name ({serviceBus.Name}): ${serviceBus.KeyNameVariable}""
echo ""      Key value ({serviceBus.Name}): ${serviceBus.KeyVariable}""
echo ""      Connection string ({serviceBus.Name}): ${serviceBus.ConnectionStringVariable}""";

        public string GenerateEnvScript() => $@"IFS=$'\t' read -r {serviceBus.ConnectionStringVariable} {serviceBus.KeyVariable} <<< `az servicebus namespace authorization-rule keys list -g ""${serviceBus.ResourceGroup.ResourceGroupNameVariable}"" --namespace-name ""${serviceBus.ServiceBusVariable}"" -n ""${serviceBus.KeyNameVariable}"" -o tsv --query ""{{connectionString: primaryConnectionString, primaryKey: primaryKey}}""`";
    }
}
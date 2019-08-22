namespace Provision
{
    class EventHubNamespace : IResource
    {
        public string Location { get; set; }
        public string Name { get; set; } = "default";
        public string SKU { get; set; } = "Basic";
        public string ThroughputUnits { get; set; } = "1";
        public string Capacity { get; set; } = "1";
        public string EnableAutoInflate { get; set; } = "false";
        public string EnableKafka { get; set; } = "false";
        public string EventHubNamespaceVariable { get; set; }
        public string EventHubNamespaceName { get; set; }
        public ResourceGroup ResourceGroup { get; set; }
        public EventHubNamespace(Context context)
        {
            this.Location = $"${context.LocationVariable}";
            if (string.IsNullOrEmpty(this.EventHubNamespaceVariable))
                this.EventHubNamespaceVariable = Name.ToUpper() + "_EVENTHUB_NAMESPACE";
            if (string.IsNullOrEmpty(this.EventHubNamespaceName))
                this.EventHubNamespaceName = $"${context.BaseNameVariable}-${context.Random5charBaseVariable}";
        }

        public int Order { get; set; } = 2;
    }
}
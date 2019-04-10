namespace Provision
{
    class EventGridSubscription : IResource
    {
        public string Name { get; set; } = "default";
        public string EventGridSubscriptionName { get; set; }
        public string Endpoint { get; set; } = null;
        public string EndpointType { get; set; } = "webhook";
        private string eventGridSubscriptionVariableName = null;
        public string SourceResourceId { get; set; } = "$SUBSCRIPTION_RESOURCE_ID";
        public string[] IncludedEventTypes { get; set; }
        public string EventGridSubscriptionVariableName
        {
            get => eventGridSubscriptionVariableName ?? this.Name.ToUpper() + "_EVENT_GRID_SUBSCRIPTION";
            set => eventGridSubscriptionVariableName = value;
        }
        private ResourceGroup resourceGroup = null;
        [Dependency(Optional = true, Description = "If set, source resource id will be set to that resource group")]
        public ResourceGroup ResourceGroup
        {
            get => resourceGroup;
            set
            {
                resourceGroup = value;
                SourceResourceId = "$" + resourceGroup.ResourceGroupResourceIdVariable;
            }
        }
        private void setResourceIdForQueue()
        {
            SourceResourceId = "$" + storageAccount?.StorageAccountResourceIdVariableName + "/queueServices/default/queues/" + Queue;
        }
        public string Queue { 
            get => queue; 
            set {
                queue = value;
                setResourceIdForQueue();
            } 
        }
        private StorageAccount storageAccount = null;
        private string queue = null;

        [Dependency(Optional = true, Description = "If specified, and a queue name is provided, will use the storage queue as the subscription target.")]
        public StorageAccount StorageAccount
        {
            get => storageAccount;
            set
            {
                storageAccount = value;
                setResourceIdForQueue();
            }
        }

        public EventGridSubscription(Context context)
        {
            EventGridSubscriptionName = $"${context.BaseNameVariable}-`random 5`";
        }

        public int Order => 5;
    }
}
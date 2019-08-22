namespace Provision
{
    class EventHub : IResource
    {
        public string Name { get; set; } = "default";
        [Dependency(Optional = false)]
        public EventHubNamespace EventHubNamespace { get; set; }
        [Dependency(Optional = true, Description = "Eventhub will archive events to this account if specified")]
        public StorageAccount ArchiveStorageAccount { get; set; }
        public string ArchiveBlobContainer { get; set; } = "eventhub-archive";
        public string PartitionCount { get; set; }
        public string EventHubVariable { get; set; }
        public string EventHubName { get; set; }
        public string AuthorizationRuleName { get; set; } = "admin";
        public string ConnectionStringVariable { get => EventHubVariable + "_CONNECTION_STRING"; }
        public string KeyVariable { get => EventHubVariable + "_KEY"; }
        public string KeyNameVariable { get => EventHubVariable + "_KEY_NAME"; }
        public string MessageRetention { get; set; } = "1";
        public EventHub(Context context)
        {
            if (string.IsNullOrEmpty(this.EventHubVariable))
                this.EventHubVariable = Name.ToUpper() + "_EVENTHUB";
            if (string.IsNullOrEmpty(this.EventHubName))
                this.EventHubName = $"${context.BaseNameVariable}-${context.Random5charBaseVariable}";
        }

        public int Order { get; set; } = 3;
    }
}
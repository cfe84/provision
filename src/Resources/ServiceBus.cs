using System.Collections.Generic;

namespace Provision
{
    internal class ServiceBus : IResource
    {

        public string Location { get; set; }
        public string Name { get; set; } = "default";
        public string SKU { get; set; } = "Basic";
        [Dependency(Optional = true, Description = "If SKU is Premium, capacity")]
        public string Capacity { get; set; } = "1";
        public string ServiceBusVariable { get; set; }
        public string ServiceBusName { get; set; }
        public string ConnectionStringVariable { get => ServiceBusVariable + "_CONNECTION_STRING"; }
        public string KeyVariable { get => ServiceBusVariable + "_KEY"; }
        public string KeyNameVariable { get => ServiceBusVariable + "_KEY_NAME"; }
        public ResourceGroup ResourceGroup { get; set; }
        public string[] Topics { get; set; } = new string[0];
        [Dependency(Optional = true, Description = "Will create the subscriptions on the first topic")]
        public string[] Subscriptions { get; set; } = new string[0];
        public string[] Queues { get; set; } = new string[0];
        public ServiceBus(Context context)
        {
            this.Location = $"${context.LocationVariable}";
            if (string.IsNullOrEmpty(this.ServiceBusVariable))
                this.ServiceBusVariable = Name.ToUpper() + "_SERVICE_BUS";
            if (string.IsNullOrEmpty(this.ServiceBusName))
                this.ServiceBusName = $"${context.BaseNameVariable}-${context.Random5charBaseVariable}";
        }

        public int Order { get; set; } = 2;
    }
}
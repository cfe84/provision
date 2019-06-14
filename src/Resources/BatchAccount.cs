using System.Collections.Generic;

namespace Provision {
    internal class BatchAccount: IResource {
        
        public string Location {get; set;}
        public string Name {get; set;} = "DEFAULT";
        public string BatchAccountNameVariable => Name + "_BATCH_ACCOUNT_NAME";
        public string BatchAccountEndpointVariable => Name + "_BATCH_ACCOUNT_ENDPOINT";
        public string BatchAccountKeyVariable => Name + "_BATCH_ACCOUNT_KEY";
        public string BatchPoolNameVariable => Name + "_BATCH_POOL_NAME";
        public string BatchPoolName {get ;set ;} = "default-pool";
        public string BatchApplicationNameVariable => Name + "_BATCH_APPLICATION_NAME";
        public string BatchApplicationVersionVariable => Name + "_BATCH_APPLICATION_VERSION";
        public string BatchAccountName {get; set;}
        public string VmSize {get; set;} = "standard_d2s_v3";
        public string NodeAgentSku {get; set; } = "batch.node.ubuntu 16.04";
        public string ImagePublisher {get; set; } = "Canonical";
        public string ImageOffer {get; set;} = "UbuntuServer";
        public string ImageSku {get; set; } = "16.04.0-LTS";
        public string TargetDedicatedNodes {get; set;} = "1";
        public string TargetLowPriorityNodes {get; set;} = "0";
        public string StartTask {get; set; }
        public string ApplicationName {get; set;} = "default-application";
        public string ApplicationVersion {get; set;} = "1";
        [Dependency(Optional = true, Description = "If specified, folder containining the application package. It will be zipped and uploaded")]
        public string ApplicationFolder {get; set;}
        [Dependency(Optional = true, Description = "If specified, zip containining the application package. It will be uploaded")]
        public string ApplicationZip {get; set;}
        public StorageAccount StorageAccount {get; set;}
        public ResourceGroup ResourceGroup {get; set;}
        public BatchAccount(Context context)
        {
            this.Location = $"${context.LocationVariable}";
            this.BatchAccountName = $"{context.StripAndLowercaseResourceName($"${context.BaseNameVariable}-bat-${context.Random5charBaseVariable}", 24)}";
        }

        public int Order {get;set;} = 3;
    }
}
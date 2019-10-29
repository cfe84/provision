using System.Collections.Generic;

namespace Provision
{
    internal class StorageAccount : IResource
    {

        public string Location { get; set; }
        public string Name { get; set; } = "default";
        private string storageAccountVariableName = null;
        public string StorageAccountVariableName
        {
            get => storageAccountVariableName ?? this.Name.ToUpper() + "_STORAGE_ACCOUNT";
            set => storageAccountVariableName = value;
        }
        public string StorageAccountResourceIdVariableName
        {
            get => StorageAccountVariableName + "_RESOURCE_ID";
        }
        public string SKU { get; set; } = "Standard_LRS";
        private string accountPostfix = null;
        public string AccountPostfix
        {
            get => accountPostfix ?? Name.Substring(0, 3);
            set => accountPostfix = value;
        }
        public string ConnectionStringVariableName { get => StorageAccountVariableName + "_CONNECTION_STRING"; }
        public ResourceGroup ResourceGroup { get; set; }
        public string Kind { get; set; } = "StorageV2";
        public string[] Containers { get; set; } = new string[0];
        public string[] Queues { get; set; } = new string[0];
        public string[] Tables { get; set; } = new string[0];

        public string CorsMethods { get; set; } = null;

        public StorageAccount(Context context)
        {
            this.Location = $"${context.LocationVariable}";
        }

        public int Order { get; set; } = DEFAULT_ORDER;
        public const int DEFAULT_ORDER = 2;
    }
}
namespace Provision
{
    class BlobContainer : IResource
    {
        [Dependency(Optional = false)]
        public StorageAccount StorageAccount { get; set; }
        private int order = -1;
        public int Order
        {
            get => order < 0 ? StorageAccount.Order + 1 : order;
            set => order = value;
        }

        public string Name { get; set; } = "default";
        private string variable = null;
        public string Variable
        {
            get => variable ?? this.Name.ToUpper() + "_CONTAINER";
            set => variable = value;
        }

        private string containerName;

        public BlobContainer(Context context)
        {
        }

        public string ContainerName
        {
            get => containerName ?? this.Name;
            set => containerName = value;
        }
    }
}
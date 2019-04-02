namespace Provision {
    class StorageAccountReference : IReference
    {
        public string Name => "StorageAccount";

        public IResource CreateResource(Context context)
        {
            return new StorageAccount(context);
        }

        public IResourceGenerator CreateResourceGenerator(IResource resource)
        {
            return new StorageAccountGenerator((StorageAccount)resource);
        }

        public IResourceParser GetParser()
        {
            return new DefaultResourceParser<StorageAccount>();
        }
    }
}
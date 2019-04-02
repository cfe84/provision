namespace Provision {
    class StorageAccountReference : IReference
    {
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
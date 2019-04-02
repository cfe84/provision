namespace Provision {
    class ResourceGroupReference : IReference
    {
        public IResourceGenerator CreateResourceGenerator(IResource resource)
        {
            return new ResourceGroupGenerator((ResourceGroup)resource);
        }

        public IResourceParser GetParser()
        {
            return new DefaultResourceParser<ResourceGroup>();
        }
    }
}
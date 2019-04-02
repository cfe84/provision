namespace Provision {
    class ResourceGroupReference : IReference
    {
        public string Name => "ResourceGroup";

        public IResource CreateResource(Context context)
        {
            return new ResourceGroup(context);
        }

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
namespace Provision {
    class WebAppReference : IReference
    {
        public IResourceGenerator CreateResourceGenerator(IResource resource)
        {
            return new WebAppGenerator((WebApp)resource);
        }

        public IResourceParser GetParser()
        {
            return new DefaultResourceParser<WebApp>();
        }
    }
}
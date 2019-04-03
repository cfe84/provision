namespace Provision {
    class AppServicePlanReference : IReference
    {
        public IResourceGenerator CreateResourceGenerator(IResource resource)
        {
            return new AppServicePlanGenerator((AppServicePlan)resource);
        }

        public IResourceParser GetParser()
        {
            return new DefaultResourceParser<AppServicePlan>();
        }
    }
}
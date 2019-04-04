using System;
using System.Reflection;

namespace Provision {
    class GenericReference : IReference
    {
        private readonly Type resourceType;
        private readonly ConstructorInfo resourceGeneratorConstructor;

        public GenericReference(Type resourceType, ConstructorInfo resourceGeneratorConstructor)
        {
            this.resourceType = resourceType;
            this.resourceGeneratorConstructor = resourceGeneratorConstructor;
        }
        public IResourceGenerator CreateResourceGenerator(IResource resource)
        {
            return (IResourceGenerator)resourceGeneratorConstructor.Invoke(new [] {resource});
        }

        public IResourceParser GetParser()
        {
            return new DefaultResourceParser(resourceType);
        }
    }
}
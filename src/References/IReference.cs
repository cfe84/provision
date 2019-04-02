using System;

namespace Provision {
    interface IReference {
        // TODO: Think on how to make that typesafe
        IResourceGenerator CreateResourceGenerator(IResource resource); 
        IResourceParser GetParser();
    }
}
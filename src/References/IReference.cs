using System;

namespace Provision {
    interface IReference {
        IResource CreateResource(Context context);
        // TODO: Think on how to make that typesafe
        IResourceGenerator CreateResourceGenerator(IResource resource); 
        IResourceParser GetParser();
    }
}
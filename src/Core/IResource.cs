using System.Collections.Generic;

namespace Provision {
    internal interface IResource {
        int Order { get; }
        string Name { get; }
    }
}
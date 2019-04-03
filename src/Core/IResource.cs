using System.Collections.Generic;

namespace Provision {
    internal interface IResource {
        List<DependencyRequirement> DependencyRequirements {get;}
        int Order { get; }
        string Name { get; }
    }
}
using System.Collections.Generic;

namespace Provision {
    internal interface IResource {
        List<DependencyRequirement> DependencyRequirements {get;}
        void InjectDependency(string name, IResource value);
        int Order { get; }
        string Name { get; }
    }
}
using System.Collections.Generic;

namespace Provision
{
    class ParsedResource
    {
        public IResource Resource{ get; set; }
        public IEnumerable<DependencyRequirement> ExplicitDependenciesToBeInjected { get; set; }
    }
}
using System.Collections.Generic;

namespace Provision {
    internal class ResourceTree {
        public string Location { get; set;}
        public IEnumerable<ResourceSpecification> Resources {get; set;}
    }
}
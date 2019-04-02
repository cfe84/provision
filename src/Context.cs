using System;
using System.Collections.Generic;

namespace Provision {
    internal class Context {
        public string BaseNameVariable { get; set; } = "NAME";
        public string LocationVariable { get; set; } = "LOCATION";
        public string DefaultLocation {get; set; } = "westus2";
        public List<IResource> Resources {get;} = new List<IResource>();
    }
}
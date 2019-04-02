using System;

namespace Provision {
    internal class DependencyRequirement {
        public string Name {get;set;}
        public string ValueName {get;set;} = "default";
        public Type Type {get; set;}
    }
}
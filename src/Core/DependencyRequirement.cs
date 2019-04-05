using System;
using System.Reflection;

namespace Provision {
    // TODO: make an attribute
    internal class DependencyRequirement {
        public PropertyInfo Property {get;set;}
        public string ValueName {get;set;} = "default";
    }
}
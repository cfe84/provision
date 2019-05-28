using System;
using System.Collections.Generic;

namespace Provision {
    internal class Context {
        public string BaseNameVariable { get; set; } = "NAME";
        public string LocationVariable { get; set; } = "LOCATION";
        public string DefaultLocation {get; set; } = "westus2";
        public string SubscriptionIdVariable {get; set;} = "SUBSCRIPTIONID";
        public string TenantIdVariable {get; set;} = "TENANTID";
        public string Random16charBaseVariable {get; set;} = "RANDOMBASE16CHAR";
        public string Random5charBaseVariable {get; set;} = "RANDOMBASE";
        public string StripAndLowercaseResourceName(string baseName, int maxLength) {
            return $"`echo \"{baseName}\" | sed -e 's/-//g' | sed -E 's/^(.*)$/\\L\\1/g' | head -c {maxLength}`";
        }
        public List<IResource> Resources {get;} = new List<IResource>();
        public Dictionary<IResource, List<DependencyRequirement>> ExplicitDependencyRequirements {get;}
            = new Dictionary<IResource, List<DependencyRequirement>>();
    }
}
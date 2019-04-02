using System.Collections.Generic;

namespace Provision {
    internal class ResourceSpecification {
        public string ResourceType {get; set;}
        public Dictionary<string, string> StringProperties {get; set;} = new Dictionary<string, string>();
        public Dictionary<string, List<string>> ListProperties {get; set;} = new Dictionary<string, List<string>>();
    }
}
using System.Collections.Generic;

namespace Provision {
    internal class Deployment: IResource {
        public Deployment(Context context)
        {
        }
        public string Name {get; set;} = "default";
        [Dependency(Optional = false, Description = "Resource group where to deploy the deployment")]
        public ResourceGroup ResourceGroup { get; set;}
        public string File { get; set; }
        public string Uri { get; set; }
        public string[] Parameters { get; set; } = new string [0];
        public string Mode { get; set; } = "Incremental";
        public int Order => 7;
    }
}
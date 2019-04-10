using System.Collections.Generic;

namespace Provision {
    internal class ResourceGroup: IResource {
        public ResourceGroup(Context context)
        {
            this.Location = $"${context.LocationVariable}";
            this.ResourceGroupName = "$" + context.BaseNameVariable;
        }
        public string Name {get; set;} = "default";
        private string resourceGroupNameVariable = null;
        public string ResourceGroupNameVariable {get => resourceGroupNameVariable ?? Name.ToUpper() + "_RESOURCE_GROUP"; }
        public string ResourceGroupResourceIdVariable { get => ResourceGroupNameVariable + "_RESOURCE_ID"; }
        public string ResourceGroupName { get; set; }
        public string Location { get; set; }
        public int Order => 1;
    }
}
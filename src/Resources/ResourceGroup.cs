using System.Collections.Generic;

namespace Provision {
    internal class ResourceGroup: IResource {
        public ResourceGroup(Context context)
        {
            this.Location = $"${context.LocationVariable}";
            this.ResourceGroupName = "$" + context.BaseNameVariable;
        }

        public void InjectDependency(string name, IResource value)
        {
            
        }


        public string Name {get; set;} = "default";
        private string resourceGroupNameVariable = null;
        public string ResourceGroupNameVariable {get => resourceGroupNameVariable ?? Name.ToUpper() + "_RESOURCE_GROUP"; }
        public string ResourceGroupName { get; set; }
        public string Location { get; set; }
        public int Order => 1;

        public List<DependencyRequirement> DependencyRequirements => new List<DependencyRequirement>();
    }
}
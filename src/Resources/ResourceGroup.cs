using System.Collections.Generic;

namespace Provision {
    internal class ResourceGroup: IResource {
        public ResourceGroup(Context context, string name = "default", string resourceGroupName = null)
        {
            this.Name = name;
            this.ResourceGroupNameVariable = name.ToUpper() + "_RESOURCE_GROUP";
            this.Location = context.LocationVariable;
            this.ResourceGroupName = resourceGroupName ?? "$" + context.BaseNameVariable;
        }

        public void InjectDependency(string name, IResource value)
        {
            
        }

        public IResourceGenerator GetGenerator() => new ResourceGroupGenerator(this);

        public string Name {get; private set;}
        public string ResourceGroupNameVariable {get; private set;}
        public string ResourceGroupName;
        public string Location;
        public int Order => 1;

        public IEnumerable<DependencyRequirement> DependencyRequirements => new DependencyRequirement[]{};
    }
}
using System.Collections.Generic;

namespace Provision {
    internal class StorageAccount: IResource {
        
        public string LocationVariable {get; private set;}
        public string Name {get; private set;}
        public string StorageAccountVariableName {get; private set;}
        public string AccountPostfix { get; private set; }
        public string ConnectionStringVariableName { get; private set;}
        public ResourceGroup ResourceGroup {get; private set;}
        public string [] Containers {get; set; }
        
        public StorageAccount(Context context, 
            string name = "default",
            string accountPostfix = "", 
            string[] containers = null)
        {
            this.AccountPostfix = accountPostfix;
            this.Name = name;
            this.StorageAccountVariableName = name.ToUpper() + "_STORAGE_ACCOUNT";
            this.ConnectionStringVariableName = StorageAccountVariableName + "_CONNECTION_STRING";
            this.Containers = containers ?? new string[0];
            this.LocationVariable = context.LocationVariable;
        }

        public int Order => 2;

        public IEnumerable<DependencyRequirement> dependencyRequirements = 
            DependencyUtils.CreateDefaultDependencyRequirementForType(new [] { typeof(ResourceGroup)});

        public IEnumerable<DependencyRequirement> DependencyRequirements => dependencyRequirements;

        public void InjectDependency(string name, IResource value)
        {
            if (value.GetType() == typeof(ResourceGroup)) {
                this.ResourceGroup = (ResourceGroup)value;
            }
        }

        public IResourceGenerator GetGenerator() => new StorageAccountGenerator(this);
    }
}
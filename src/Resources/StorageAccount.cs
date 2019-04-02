using System.Collections.Generic;

namespace Provision {
    internal class StorageAccount: IResource {
        
        public string Location {get; set;}
        public string Name {get; set;} = "default";
        private string storageAccountVariableName = null;
        public string StorageAccountVariableName {
            get => storageAccountVariableName ?? this.Name.ToUpper() + "_STORAGE_ACCOUNT";
            set => storageAccountVariableName = value;
        }
        public string AccountPostfix { get; set; } = "";
        public string ConnectionStringVariableName { get => StorageAccountVariableName + "_CONNECTION_STRING"; }
        public ResourceGroup ResourceGroup {get; private set;}
        public string [] Containers {get; set; } =  new string[0];
        
        public StorageAccount(Context context)
        {
            this.Location = $"${context.LocationVariable}";
        }

        public int Order => 2;

        public List<DependencyRequirement> dependencyRequirements = 
            DependencyUtils.CreateDefaultDependencyRequirementForType(new [] { typeof(ResourceGroup)});

        public List<DependencyRequirement> DependencyRequirements => dependencyRequirements;

        public void InjectDependency(string name, IResource value)
        {
            if (value.GetType() == typeof(ResourceGroup)) {
                this.ResourceGroup = (ResourceGroup)value;
            }
        }

        public IResourceGenerator GetGenerator() => new StorageAccountGenerator(this);
    }
}
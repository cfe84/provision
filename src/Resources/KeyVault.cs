using System.Collections.Generic;

namespace Provision {
    internal class KeyVault: IResource {
        
        public ResourceGroup ResourceGroup {get; set;}
        
        [Dependency(Optional = true, Description = "Specify a service principal that will receive permissions to this KeyVault")]
        public ServicePrincipal ServicePrincipal { get; set; }

        public string Location {get; set;}
        public string Name {get; set;} = "default";
        private string keyVaultVariableName = null;
        public string KeyVaultVariableName {
            get => keyVaultVariableName ?? this.Name.ToUpper() + "_KEYVAULT";
            set => keyVaultVariableName = value;
        }
        public string KeyVaultName { get; set; }
        public KeyVault(Context context)
        {
            this.Location = $"${context.LocationVariable}";
            this.KeyVaultName = $"`echo ${context.BaseNameVariable} | head -c 19``random 5`";
        }

        public int Order {get; set;} = 2;
    }
}
using System.Collections.Generic;

namespace Provision {
    internal class ServicePrincipal: IResource {
        
        public string Name {get; set;} = "default";
        public string ApplicationName { get; set; } 
        private string applicationIdentifierUriVariable = null;
        public string ApplicationIdentifierUriVariable {
            get => applicationIdentifierUriVariable ?? this.Name.ToUpper() + "_SERVICE_PRINCIPAL_IDENTIFIER_URI";
            set => applicationIdentifierUriVariable = value;
        }
        private string applicationIdVariable = null;
        public string ApplicationIdVariable {
            get => applicationIdVariable ?? this.Name.ToUpper() + "_SERVICE_PRINCIPAL_ID";
            set => applicationIdVariable = value;
        }
        private string passwordVariable = null;
        public string PasswordVariable { 
            get => passwordVariable ?? this.Name.ToUpper() + "_SERVICE_PRINCIPAL_PASSWORD";
            set => passwordVariable = value; 
        }
        
        public ServicePrincipal(Context context)
        {
            ApplicationName = $"http://${context.BaseNameVariable}/`random 5`";
        }

        public int Order => 1;
    }
}
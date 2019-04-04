using System.Collections.Generic;

namespace Provision {
    internal class ServicePrincipal: IResource {
        
        public string Name {get; set;} = "default";
        public string ApplicationName { get; set; } 
        private string applicationIdentifierUriVariable = null;
        public string ApplicationIdentifierUriVariable {
            get => applicationIdentifierUriVariable ?? this.Name.ToUpper() + "_APPLICATION_IDENTIFIER_URI";
            set => applicationIdentifierUriVariable = value;
        }
        private string applicationIdVariable = null;
        public string ApplicationIdVariable {
            get => applicationIdVariable ?? this.Name.ToUpper() + "_APPLICATION_ID";
            set => applicationIdVariable = value;
        }
        private string passwordVariable = null;
        public string PasswordVariable { 
            get => passwordVariable ?? this.Name.ToUpper() + "_APPLICATION_PASSWORD";
            set => passwordVariable = value; 
        }
        
        public ServicePrincipal(Context context)
        {
            ApplicationName = $"http://${context.BaseNameVariable}/`random 5`";
        }

        public int Order => 1;

        public List<DependencyRequirement> dependencyRequirements = 
            new List<DependencyRequirement>();

        public List<DependencyRequirement> DependencyRequirements => dependencyRequirements;
    }
}
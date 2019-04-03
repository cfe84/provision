using System.Collections.Generic;

namespace Provision {
    internal class WebApp: IResource {
        
        public string Name {get; set;} = "default";
        public string WebAppName {get; set;}
        public string[] Settings {get; set;} = null;
        private string webAppVariableName = null;
        public string WebAppVariableName {
            get => webAppVariableName ?? this.Name.ToUpper() + "_WEB_APP";
            set => webAppVariableName = value;
        }
        private string hostNameVariable = null;
        public string HostNameVariable {
            get => hostNameVariable ?? this.WebAppVariableName.ToUpper() + "_HOSTNAME";
            set => hostNameVariable = value;
        }
        private string gitUrlVariable = null;
        public string GitUrlVariable {
            get => gitUrlVariable ?? this.WebAppVariableName.ToUpper() + "_GIT_URL";
            set => gitUrlVariable = value;
        }
        public string Deploy {get; set;} = "false";
        public ResourceGroup ResourceGroup {get; set;}
        public AppServicePlan AppServicePlan {get; set;}
        
        public WebApp(Context context)
        {
            WebAppName = $"${context.BaseNameVariable}-`random 5`";
        }

        public int Order => 4;

        public List<DependencyRequirement> dependencyRequirements = 
            DependencyUtils.CreateDefaultDependencyRequirementForType(new [] { typeof(AppServicePlan), typeof(ResourceGroup)});

        public List<DependencyRequirement> DependencyRequirements => dependencyRequirements;
    }
}
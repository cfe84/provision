using System.Collections.Generic;

namespace Provision {
    internal class AppServicePlan: IResource {
        
        public string Location {get; set;}
        public string Name {get; set;} = "default";
        private string deploymentUserName = null;
        public string DeploymentUserName {
            get => deploymentUserName ?? $"${AppServicePlanAccountVariableName}";
            set => deploymentUserName = value;
            }
        private string deploymentPassword = null;
        public string DeploymentPassword {
            get => deploymentPassword ?? "`random 30`";
            set => deploymentPassword = value;
            }
        
        private string appServicePlanVariableName = null;
        public string AppServicePlanAccountVariableName {
            get => appServicePlanVariableName ?? this.Name.ToUpper() + "_APP_SERVICE_PLAN";
            set => appServicePlanVariableName = value;
        }
        public ResourceGroup ResourceGroup {get; set;}
        public string SKU {get; set;} = "FREE";
        public AppServicePlan(Context context)
        {
            this.Location = $"${context.LocationVariable}";
        }

        public int Order => 3;
    }
}
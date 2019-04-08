using System.Collections.Generic;

namespace Provision {
    internal class FunctionApp: IResource {
        
        public string Name {get; set;} = "default";
        public string FunctionAppName {get; set;}
        public string Location {get; set;}
        public string[] Settings {get; set;} = null;
        private string functionAppVariableName = null;
        public string FunctionAppVariableName {
            get => functionAppVariableName ?? this.Name.ToUpper() + "_FUNCTIONAPP";
            set => functionAppVariableName = value;
        }
        private string hostNameVariable = null;
        public string HostNameVariable {
            get => hostNameVariable ?? this.FunctionAppVariableName.ToUpper() + "_HOSTNAME";
            set => hostNameVariable = value;
        }
        public string Deploy {get; set;} = "false";
        public ResourceGroup ResourceGroup {get; set;}
        [Dependency(Optional = true, Description = "If specified, will create the functionapp in an app service plan")]
        public AppServicePlan AppServicePlan {get; set;}
        [Dependency(Optional = false, Description = "Storage account used for functions inners")]
        public StorageAccount StorageAccount {get; set;}
        public string OsType {get; set;} = "Windows";
        
        public FunctionApp(Context context)
        {
            Location = $"{context.LocationVariable}";
            FunctionAppName = $"${context.BaseNameVariable}-`random 5`";
        }

        public int Order => 4;
    }
}
using System.Collections.Generic;

namespace Provision {
    internal class WebApp : IResource
    {
        public string TenantId {get ;set;}
        public string Name { get; set; } = "default";
        public string WebAppName { get; set; }
        public string[] Settings { get; set; } = null;
        private string webAppVariableName = null;
        public string WebAppVariableName
        {
            get => webAppVariableName ?? this.Name.ToUpper() + "_WEB_APP";
            set => webAppVariableName = value;
        }
        private string hostNameVariable = null;
        public string HostNameVariable
        {
            get => hostNameVariable ?? this.WebAppVariableName.ToUpper() + "_HOSTNAME";
            set => hostNameVariable = value;
        }
        private string gitUrlVariable = null;

        public string GitUrlVariable
        {
            get => gitUrlVariable ?? this.WebAppVariableName.ToUpper() + "_GIT_URL";
            set => gitUrlVariable = value;
        }
        public string Deploy { get; set; } = "false";
        public ResourceGroup ResourceGroup { get; set; }
        public AppServicePlan AppServicePlan { get; set; }
        [Dependency(Optional=true, Description="If specified, will use this registration for easyauth")]
        public AppRegistration AppRegistration { get ; set; }
        public WebApp(Context context)
        {
            TenantId = $"${context.TenantIdVariable}";
            WebAppName = $"${context.BaseNameVariable}-${context.Random5charBaseVariable}";
        }

        public int Order => 4;
    }
}
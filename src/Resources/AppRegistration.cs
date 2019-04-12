namespace Provision {
    class AppRegistration: IResource
    {
        public string Name { get; set; } = "default";
        public string ApplicationName { get; set; }
        public string IdentifierUri => "https://" + ApplicationName;
        private string applicationIdentifierUriVariable = null;
        public string ApplicationIdentifierUriVariable
        {
            get => applicationIdentifierUriVariable ?? this.Name.ToUpper() + "_APPLICATION_IDENTIFIER_URI";
            set => applicationIdentifierUriVariable = value;
        }
        private string applicationIdVariable = null;
        public string ApplicationIdVariable
        {
            get => applicationIdVariable ?? this.Name.ToUpper() + "_APPLICATION_ID";
            set => applicationIdVariable = value;
        }
        private string passwordVariable = null;

        public string PasswordVariable
        {
            get => passwordVariable ?? this.Name.ToUpper() + "_APPLICATION_PASSWORD";
            set => passwordVariable = value;
        }

        public string AvailableToOtherTenants { get; set; } = "false";

        public string ReplyUrl { get; set; }

        private FunctionApp functionApp;

        [Dependency(Optional = true, Description = "If set, will use the function app easy auth as a reply url")]
        public FunctionApp FunctionApp { 
            get => functionApp; 
            set {
                functionApp = value; 
                ReplyUrl = $"${functionApp.HostNameVariable}/.auth/login/aad/callback";
            }
        }

        private WebApp webApp;
        [Dependency(Optional = true, Description = "If set, will use the webapp easy auth as a reply url")]
        public WebApp WebApp { 
            get => webApp; 
            set {
                webApp = value;
                ReplyUrl = $"${webApp.HostNameVariable}/.auth/login/aad/callback";
            }
        }

        public AppRegistration(Context context)
        {
            ApplicationName = $"${context.BaseNameVariable}-`random 5`";
        }

        public int Order => 1;
    }
}
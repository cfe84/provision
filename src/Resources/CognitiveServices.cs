using System.Collections.Generic;

namespace Provision {
    internal class CognitiveServices: IResource {
        
        public string Name {get; set;} = "default";
        public string Location {get; set;}
        private string cognitiveServicesAccountNameVariable = null;
        public string CognitiveServicesAccountNameVariable {
            get => cognitiveServicesAccountNameVariable ?? this.Name.ToUpper() + "_COGNITIVE_SERVICE";
            set => cognitiveServicesAccountNameVariable = value;
        }
        private string cognitiveServicesKeyVariable = null;
        public string CognitiveServicesKeyVariable {
            get => cognitiveServicesKeyVariable ?? this.CognitiveServicesAccountNameVariable + "_KEY";
            set => cognitiveServicesKeyVariable = value;
        }
        private string cognitiveServicesEndpointVariable = null;
        public string CognitiveServicesEndpointVariable {
            get => cognitiveServicesEndpointVariable ?? this.CognitiveServicesAccountNameVariable + "_ENDPOINT";
            set => cognitiveServicesEndpointVariable = value;
        }
        public string CognitiveServicesName {get;set;}
        public ResourceGroup ResourceGroup {get; set;}
        public string Sku {get; set;} = "S0";
        public string Kind {get; set;} = "Face";
        
        public CognitiveServices(Context context)
        {
            Location = $"{context.LocationVariable}";
            CognitiveServicesName = $"${context.BaseNameVariable}-`random 5`";
        }

        public int Order => 3;
    }
}
namespace Provision {
    internal class EventGridSubscriptionGenerator : IResourceGenerator
    {
        private EventGridSubscription eventGridSubscription;
        public EventGridSubscriptionGenerator(EventGridSubscription eventGridSubscription) {
            this.eventGridSubscription = eventGridSubscription;
        }
        
        public string GenerateResourceNameDeclaration()
            => $"{eventGridSubscription.EventGridSubscriptionVariableName}=\"{eventGridSubscription.EventGridSubscriptionName}\"";

        public string GenerateCleanupScript() 
            => "";

        private string generateIncludedEventTypesOption() =>
        eventGridSubscription.IncludedEventTypes != null ? " --included-event-types " + string.Join(" ", eventGridSubscription.IncludedEventTypes) + " " : "";

        public string GenerateProvisioningScript() 
            => 
$@"echo ""Creating event grid subscription ${eventGridSubscription.EventGridSubscriptionVariableName}""
az eventgrid event-subscription create --name ${eventGridSubscription.EventGridSubscriptionVariableName} --endpoint {eventGridSubscription.Endpoint} --endpoint-type {eventGridSubscription.EndpointType} --source-resource-id {eventGridSubscription.SourceResourceId}"
        + generateIncludedEventTypesOption();

        public string GenerateSummary() 
            => $@"echo ""    Event grid name: ${eventGridSubscription.EventGridSubscriptionVariableName}""";

    }
}
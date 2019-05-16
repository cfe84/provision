using System;
using System.Linq;
using System.Collections.Generic;

namespace Provision {

    class ParserException: FunctionalException {
        public ParserException(string message): base(message) {}
    }

    internal static class Parser {

        public static Context Parse(ResourceTree resourceTree) {
            var context = new Context();
            if (resourceTree.Location != null)
                context.DefaultLocation = resourceTree.Location;

            foreach(ResourceSpecification resourceSpecification in resourceTree.Resources) {
                IResourceParser parser = Resolver
                    .GetReference(resourceSpecification.ResourceType)
                    .GetParser();
                ParsedResource resource = parser.ParseResourceSpecification(context, resourceSpecification);
                context.Resources.Add(resource.Resource);
                context.ExplicitDependencyRequirements.Add(resource.Resource, resource.ExplicitDependenciesToBeInjected.ToList());
            }

            return context;
        }
    }
}
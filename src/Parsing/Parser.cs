using System;
using System.Collections.Generic;

namespace Provision {

    public class ParserException: Exception {
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
                IResource resource = parser.ParseResourceSpecification(context, resourceSpecification);
                context.Resources.Add(resource);
            }

            return context;
        }
    }
}
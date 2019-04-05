namespace Provision {
    interface IResourceParser {
        ParsedResource ParseResourceSpecification(Context context, ResourceSpecification specification);
    }
}
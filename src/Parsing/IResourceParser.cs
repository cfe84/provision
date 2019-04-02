namespace Provision {
    interface IResourceParser {
        IResource ParseResourceSpecification(Context context, ResourceSpecification specification);
    }
}
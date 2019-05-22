namespace Provision
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    sealed class ResourceAttribute : System.Attribute
    {
        public ResourceAttribute()
        {}

        public string Description { get; set; } = "";
    }
}
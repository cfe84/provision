namespace Provision
{
    [System.AttributeUsage(System.AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    sealed class DependencyAttribute : System.Attribute
    {
        public DependencyAttribute()
        {}

        public bool Optional { get; set; } = false;

        public string Description { get; set; } = "";
    }
}
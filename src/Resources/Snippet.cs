namespace Provision {
    [Resource(Description = "Include a code snippet in the output script")]
    internal class Snippet : IResource
    {
        public Snippet(Context context) {

        }
        [Dependency(Optional = true, Description = "Determines when to execute the snippet. Default is at the end.")]
        public int Order {get; set;} = 9;
        public string Name {get; set;}
        [Dependency(Optional = true, Description = "Snippet to include in the variable declaration part of the script")]
        public string Declaration {get; set;} = "";
        [Dependency(Optional = true, Description = "Snippet to include in the provisioning part of the script")]
        public string Provisioning {get; set;} = "";
        [Dependency(Optional = true, Description = "Snippet to include in the summary part of the script")]
        public string Summary {get; set;} = "";
        [Dependency(Optional = true, Description = "Snippet to include in the cleanup script")]
        public string Cleanup {get; set;} = "";
    }
}
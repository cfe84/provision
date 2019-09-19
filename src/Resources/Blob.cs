using System.Text.RegularExpressions;

namespace Provision
{
    class Blob : IResource
    {
        [Dependency(Optional = false, Description = "Container to which to upload the file")]
        public BlobContainer BlobContainer { get; set; }
        private int order = -1;
        public int Order
        {
            get => order < 0 ? BlobContainer.Order + 1 : order;
            set => order = value;
        }
        public string Source { get => source; set => source = value; }
        public string Target { get => target ?? Source; set => target = value; }
        private string source = "file.txt", target, name, sourceVariable, targetVariable;

        public Blob(Context context)
        {
        }
        Regex rgx = new Regex("[^a-zA-Z0-9 -]");
        public string Name { get => name ?? rgx.Replace(Source, "_"); set => name = value; }
        public string SourceVariable { get => sourceVariable ?? Name.ToUpper() + "_SOURCE"; set => sourceVariable = value; }
        public string TargetVariable { get => targetVariable ?? Name.ToUpper() + "_TARGET"; set => targetVariable = value; }
    }
}
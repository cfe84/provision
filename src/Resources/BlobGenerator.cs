namespace Provision
{
    class BlobGenerator : IResourceGenerator
    {
        private readonly Blob blob;

        public BlobGenerator(Blob blob)
        {
            this.blob = blob;
        }

        public string GenerateCleanupScript() => "";

        public string GenerateEnvScript() => "";

        public string GenerateProvisioningScript() => $@"echo ""Uploading ${blob.SourceVariable}""
if [ -d ""${blob.SourceVariable}"" ]; then
    {blob.SourceVariable}_LENGTH=`echo ""${blob.SourceVariable}"" | wc -c`
    for FILE in ""${blob.SourceVariable}""/**/*; do
        echo ""$FILE to ${blob.TargetVariable}/${{FILE:{blob.SourceVariable}_LENGTH}}""
    done
elif [ -f ""${blob.SourceVariable}"" ]; then
    echo ""Single file: ${blob.SourceVariable} to ${blob.TargetVariable}/$(basename ""${blob.SourceVariable}"")""
else
    echo ""Not found: ${blob.SourceVariable}""
fi";

        public string GenerateResourceNameDeclaration() => $@"{blob.SourceVariable}=""{blob.Source}""
{blob.TargetVariable}=""{blob.Target}""";

        public string GenerateSummary() => "";
    }
}
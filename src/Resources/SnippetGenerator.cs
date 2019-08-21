namespace Provision
{
    class SnippetGenerator : IResourceGenerator
    {
        Snippet snippet;
        public SnippetGenerator(Snippet snippet) => this.snippet = snippet;
        private string EscapeString(string str) => str.Replace("\\", "\\\\").Replace("\"", "\\\"");
        public string GenerateCleanupScript() => EscapeString(this.snippet.Cleanup);
        public string GenerateProvisioningScript() => snippet.Provisioning;
        public string GenerateResourceNameDeclaration() => snippet.Declaration;
        public string GenerateSummary() => snippet.Summary;
        public string GenerateEnvScript() => snippet.Env;
    }
}
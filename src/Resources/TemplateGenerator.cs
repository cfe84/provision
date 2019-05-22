using System.Linq;

namespace Provision {
    internal class TemplateGenerator : IResourceGenerator
    {
        Template template;
        public TemplateGenerator(Template template)
        {
            this.template = template;
        }
        public string GenerateCleanupScript() => $@"echo 'Removing {template.Name} ({template.OutputFile})'
rm {template.OutputFile}";

        private string GenerateEscapedVariables() => string.Join("\n", template.Variables.Select(variable => 
            $"ESCAPED_{variable}=`echo ${variable} | sed -e 's/\\//\\\\\\//g'`"
        ));

        private string GenerateSeds() => string.Join(" |\n", template.Variables.Select(variable => 
            $"  sed \"s/_{variable}_/$ESCAPED_{variable}/g\""));

        public string GenerateProvisioningScript() => $@"echo ""Generating {template.Name} from template""
" + GenerateEscapedVariables()
+ $@"
cat {template.TemplateFile} |
" + GenerateSeds() + $@" > {template.OutputFile}";

        public string GenerateResourceNameDeclaration() => "";

        public string GenerateSummary() => $"echo \"    File ({template.Name}): {template.OutputFile}\"";
    }
}
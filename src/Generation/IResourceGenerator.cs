using System.Collections.Generic;

namespace Provision
{
    internal interface IResourceGenerator
    {
        string GenerateResourceNameDeclaration();
        string GenerateProvisioningScript();
        string GenerateEnvScript();
        string GenerateCleanupScript();
        string GenerateSummary();
    }
}
using System.Collections.Generic;

namespace Provision {
    internal interface IResourceGenerator
    {
        string GenerateResourceNameDeclaration();
        string GenerateProvisioningScript();
        string GenerateCleanupScript();
        string GenerateSummary();
    }
}
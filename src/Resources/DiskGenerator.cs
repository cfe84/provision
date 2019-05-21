using System.Collections.Generic;
using System.Linq;

namespace Provision {
    internal class DiskGenerator : IResourceGenerator
    {
        private Disk disk;
        public DiskGenerator(Disk disk) {
            this.disk = disk;
        }

        public string GenerateCleanupScript() => "";

        public string GenerateProvisioningScript() => $@"echo ""Creating disk ${disk.DiskVariableName}""
az disk create --resource-group ${disk.ResourceGroup.ResourceGroupNameVariable} --name ${disk.DiskVariableName} --location {disk.Location} --sku {disk.SKU} --size-gb {disk.SizeGb} --os-type {disk.OsType} --query ""provisioningState"" -o tsv";

        public string GenerateResourceNameDeclaration() => $@"{disk.DiskVariableName}=""{disk.DiskName}"";
{disk.DiskResourceIdVariableName}=""${disk.ResourceGroup.ResourceGroupResourceIdVariable}/providers/Microsoft.Compute/disks/${disk.DiskVariableName}""";

        public string GenerateSummary() => $@"echo ""         Disk ({disk.Name}): ${disk.DiskVariableName}""";
    }
}
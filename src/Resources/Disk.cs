using System.Collections.Generic;

namespace Provision {
    internal class Disk: IResource {
        
        public string Location {get; set;}
        public string Name {get; set;} = "default";
        public string DiskName {get; set;}
        private string diskVariableName = null;
        public string DiskVariableName {
            get => diskVariableName ?? this.Name.ToUpper() + "_DISK";
            set => diskVariableName = value;
        }
        [Dependency(Optional = true, Description = "Can be either of Windows or Linux, default is Windows")]
        public string OsType { get; set; } = "Windows";
        public string DiskResourceIdVariableName {
            get => DiskVariableName + "_RESOURCE_ID";
        }
        public string SKU { get; set; } = "Standard_LRS";
        public string SizeGb {get; set;} = "20";
        public ResourceGroup ResourceGroup {get; set;}
        
        public Disk(Context context)
        {
            this.Location = $"${context.LocationVariable}";
            this.DiskName = $"${context.BaseNameVariable}-{Name}";
        }

        public int Order => 2;
    }
}
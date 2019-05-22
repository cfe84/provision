using System.Collections.Generic;

namespace Provision {
    [Resource(Description = "Use a template to generate a file by replacing variables. Variables to be replaced must be formatted as: _VARIABLE_ and correspond to a variable in the output script")]
    internal class Template: IResource {
        
        public string Name {get; set;} = "default";
        public string TemplateFile { get; set; }
        public string OutputFile { get; set; }
        public string[] Variables {get; set;} = new string[0];
        public Template(Context context)
        {
        }

        public int Order => 10;
    }
}
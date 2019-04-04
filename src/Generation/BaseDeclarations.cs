using System.Collections;
using System.Collections.Generic;

namespace Provision {
    static class BaseDeclarations {
        public static string Header(Context values) =>
            $@"#!/bin/bash

PWD=`pwd`
NAME=`basename ""$PWD""`
{values.LocationVariable}=""{values.DefaultLocation}""

random() {{ size=$1; echo -n `date +%s%N | sha256sum | base64 | head -c $size`;}}

usage() {{
    echo ""Usage: `basename ""$0""` [--name $NAME] [--location ${values.LocationVariable}]""
    exit 1
}}

while [[ $# -gt 0 ]]
do
    key=""$1""
    shift

    case $key in
        -n|--name)
            NAME=""$1""
            shift
        ;;
        -l|--location)
            {values.LocationVariable}=""$1""
            shift
        ;;
        *)
            echo ""Unknown parameter: $key""
            usage
        ;;
    esac
done

RANDOMBASE=""`random 5`""
STORAGEBASENAME=""`echo -n $NAME | head -c 15`$RANDOMBASE""

";

        public static string CleanupScript(string scripts) => $@"
echo ""Generating cleanup script""
echo ""#!/bin/bash

{scripts}
"" > cleanup.sh
chmod +x cleanup.sh
        ";

        public static string Introduction(string resourceList) => $@"
echo ""This will provision the following resources: ""
{resourceList}
";
    }
}
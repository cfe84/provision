using System.Collections;
using System.Collections.Generic;

namespace Provision {
    static class BaseDeclarations {
        public static string Header(Context values, string envFileGenerator) =>
            $@"#!/bin/bash

PWD=`pwd`
random() {{ size=$1; echo -n `date +%s%N | sha256sum | base64 | head -c $size`;}}

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

{envFileGenerator}

source env.sh

usage() {{
    echo ""Usage: `basename ""$0""` [--name $NAME] [--location ${values.LocationVariable}]""
    exit 1
}}



";

        public static string AssembleEnvFile(Context values) {
            return $@"
RANDOMBASE=""`random 5`""
{values.SubscriptionIdVariable}=""`az account show --query id -o tsv`""
SUBSCRIPTION_RESOURCE_ID=""/subscriptions/${values.SubscriptionIdVariable}""
NAME='`basename ""$PWD""`'

if [ ! -f env.sh ]; then
    echo ""#!/bin/bash

NAME='$NAME'
{values.LocationVariable}=\""{values.DefaultLocation}\""
RANDOMBASE=\""$RANDOMBASE\""
RANDOMBASE16CHAR=\""`random 16`\""
STORAGEBASENAME=\""`echo -n $NAME | head -c 15`$RANDOMBASE\""
{values.SubscriptionIdVariable}=\""${values.SubscriptionIdVariable}\""
SUBSCRIPTION_RESOURCE_ID=\""$SUBSCRIPTION_RESOURCE_ID\""
{values.TenantIdVariable}=`az  account show --query tenantId -o tsv`
"" > env.sh
fi
"; 
        }

        public static string CleanupScript(string scripts) => $@"
echo ""Generating cleanup script""
echo ""#!/bin/bash
{scripts}
rm env.sh
"" > cleanup.sh
chmod +x cleanup.sh
        ";

        public static string Introduction(string resourceList) => $@"
echo ""This will provision the following resources: ""
{resourceList}
";
    }
}
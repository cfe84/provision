#!/bin/bash

PWD=`pwd`
NAME=`basename "$PWD"`
LOCATION="westus2"

random() { size=$1; echo -n `date +%s%N | sha256sum | base64 | head -c $size`;}

usage() {
    echo "Usage: `basename "$0"` [--name $NAME] [--location $LOCATION]"
    exit 1
}

while [[ $# -gt 0 ]]
do
    key="$1"
    shift

    case $key in
        -n|--name)
            NAME="$1"
            shift
        ;;
        -l|--location)
            LOCATION="$1"
            shift
        ;;
        *)
            echo "Unknown parameter: $key"
            usage
        ;;
    esac
done

RANDOMBASE="`random 5`"
STORAGEBASENAME="`echo -n $NAME | head -c 15`$RANDOMBASE"



echo "This will provision the following resources: "
echo "ServicePrincipal (default)"
echo "ResourceGroup (default)"
echo "KeyVault (default)"

DEFAULT_APPLICATION_IDENTIFIER_URI="http://$NAME/`random 5`"
DEFAULT_RESOURCE_GROUP="$NAME"
DEFAULT_KEYVAULT="`echo $NAME | head -c 19``random 5`"
echo "Creating Service Principal $DEFAULT_APPLICATION_IDENTIFIER_URI"
IFS=$'\t' read -r DEFAULT_APPLICATION_ID DEFAULT_APPLICATION_PASSWORD <<< `az ad sp create-for-rbac --name "$DEFAULT_APPLICATION_IDENTIFIER_URI" --query "{appId: appId, password: password}" -o tsv`
echo "Creating resource group $DEFAULT_RESOURCE_GROUP"
az group create --name $DEFAULT_RESOURCE_GROUP --location $LOCATION --query "properties.provisioningState" -o tsv
echo "Creating Keyvault $DEFAULT_KEYVAULT"
az keyvault create --name $DEFAULT_KEYVAULT -g $DEFAULT_RESOURCE_GROUP --location $LOCATION --query "properties.provisioningState"
echo "Granting permission"
az keyvault set-policy --name $DEFAULT_KEYVAULT --key-permissions create wrapKey unwrapKey get -g $DEFAULT_RESOURCE_GROUP --spn "$DEFAULT_APPLICATION_IDENTIFIER_URI" --query "properties.provisioningState"

echo "Generating cleanup script"
echo "#!/bin/bash

echo 'Removing service principal $DEFAULT_APPLICATION_IDENTIFIER_URI'
az ad sp delete --id $DEFAULT_APPLICATION_ID
echo 'Removing resource group $DEFAULT_RESOURCE_GROUP'
az group delete --name $DEFAULT_RESOURCE_GROUP --yes

" > cleanup.sh
chmod +x cleanup.sh
        
echo "                 App id: $DEFAULT_APPLICATION_ID"
echo "                App URI: $DEFAULT_APPLICATION_IDENTIFIER_URI"
echo "           App password: $DEFAULT_APPLICATION_PASSWORD"
echo "    Resource group name: $DEFAULT_RESOURCE_GROUP"
echo "              Vault url: https://$DEFAULT_KEYVAULT.vault.azure.net/"

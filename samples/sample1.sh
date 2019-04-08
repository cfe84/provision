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
echo "ResourceGroup (default)"
echo "StorageAccount (default)"
echo "KeyVault (default)"
echo "AppServicePlan (default)"
echo "WebApp (default)"

DEFAULT_RESOURCE_GROUP="$NAME"
DEFAULT_STORAGE_ACCOUNT="`echo "$STORAGEBASENAME" | sed -e 's/-//g' | sed -E 's/^(.*)$/\L\1/g' | head -c 20`def"
DEFAULT_KEYVAULT="`echo $NAME | head -c 19``random 5`"
DEFAULT_APP_SERVICE_PLAN="$NAME-`random 5`"
DEPLOYMENTUSERNAME="$DEFAULT_APP_SERVICE_PLAN"
DEPLOYMENTPASSWORD="`random 30`"
DEFAULT_WEB_APP="$NAME-`random 5`"
echo "Creating resource group $DEFAULT_RESOURCE_GROUP"
az group create --name $DEFAULT_RESOURCE_GROUP --location $LOCATION --query "properties.provisioningState" -o tsv
echo "Creating storage account $DEFAULT_STORAGE_ACCOUNT"
az storage account create --name $DEFAULT_STORAGE_ACCOUNT --kind StorageV2 --SKU Standard_LRS --location $LOCATION -g $DEFAULT_RESOURCE_GROUP --https-only true --query "provisioningState" -o tsv
DEFAULT_STORAGE_ACCOUNT_CONNECTION_STRING=`az storage account show-connection-string -g $DEFAULT_RESOURCE_GROUP -n $DEFAULT_STORAGE_ACCOUNT --query connectionString -o tsv`
echo "Creating container $DEFAULT_STORAGE_ACCOUNT.input"
az storage container create --name "input" --account-name $DEFAULT_STORAGE_ACCOUNT --query "created" -o tsv

echo "Creating container $DEFAULT_STORAGE_ACCOUNT.output"
az storage container create --name "output" --account-name $DEFAULT_STORAGE_ACCOUNT --query "created" -o tsv

echo "Creating Keyvault $DEFAULT_KEYVAULT"
az keyvault create --name $DEFAULT_KEYVAULT -g $DEFAULT_RESOURCE_GROUP --location $LOCATION --query "properties.provisioningState"
echo "Creating app service plan $DEFAULT_APP_SERVICE_PLAN"
az webapp deployment user set --user-name "$DEPLOYMENTUSERNAME" --password "$DEPLOYMENTPASSWORD" > /dev/null
az appservice plan create -g $DEFAULT_RESOURCE_GROUP -n $DEFAULT_APP_SERVICE_PLAN --sku FREE --location $LOCATION --query "provisioningState" -o tsv
echo "Creating webapp $DEFAULT_WEB_APP"
DEFAULT_WEB_APP_HOSTNAME=`az webapp create -g $DEFAULT_RESOURCE_GROUP -n $DEFAULT_WEB_APP --plan $DEFAULT_APP_SERVICE_PLAN --deployment-local-git --query "defaultHostName" -o tsv`
DEFAULT_WEB_APP_GIT_URL="https://$DEPLOYMENTUSERNAME:$DEPLOYMENTPASSWORD@$DEFAULT_WEB_APP.scm.azurewebsites.net/$DEFAULT_WEB_APP.git"
az webapp config appsettings set --name $DEFAULT_WEB_APP -g $DEFAULT_RESOURCE_GROUP --settings CONNECTION_STRING="$DEFAULT_STORAGE_ACCOUNT_CONNECTION_STRING" > /dev/null


echo "Generating cleanup script"
echo "#!/bin/bash

echo 'Removing resource group $DEFAULT_RESOURCE_GROUP'
az group delete --name $DEFAULT_RESOURCE_GROUP --yes




" > cleanup.sh
chmod +x cleanup.sh
        
echo "    Resource group name: $DEFAULT_RESOURCE_GROUP"
echo "  Storage account (def): $DEFAULT_STORAGE_ACCOUNT"
echo "      Storage key (def): $DEFAULT_STORAGE_ACCOUNT_CONNECTION_STRING"
echo "              Vault url: https://$DEFAULT_KEYVAULT.vault.azure.net/"
echo "       App service plan: $DEFAULT_APP_SERVICE_PLAN"
echo "        Deployment user: $DEPLOYMENTUSERNAME"
echo "    Deployment password: $DEPLOYMENTPASSWORD"
echo "            Webapp Name: $DEFAULT_WEB_APP"
echo "             Webapp URL: https://$DEFAULT_WEB_APP_HOSTNAME"
echo "         Webapp Git URL: $DEFAULT_WEB_APP_GIT_URL"


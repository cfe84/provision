Generate provisioning scripts for use with the Azure CLI.

For example:

```
provision keyvault --serviceprincipal app
```

Will generate a script that provisions a Resource Group, a Service Principal named "app" and a KeyVault.

# WHY would I do that?

To automate provisioning for ephemeral artifacts.

When experimenting with things, doing labs, constructing demos, the tendency is to go quick and dirty and create resource directly in the Azure Portal, then delete and forget about it forever. This approach is not perennial and is hardly repeatable. 

Moreover, demos or example built for other parties (e.g. customers) need to contain a portion on provisioning. The usual way is to manually deploy resources in a subscription, which is suboptimal.

On the other hand writing provisioning scripts, templates or recipes is a tedious and time-consuming task, which explains why it's not the usual default behavior.

This is where Provision plays its role: it lowers the friction of creating those provisioning scripts by automating a large portion of it. The typical use case for Provision is to generate a provisioning script, tweak it a little, and commit the result to source-control. The point being that it can be replayed easily by just executing the script.

# Install

You need .net core 2.1 to build and install it. I suggest installing it in any Linux shell - either the Azure Shell, or a local Ubuntu (WSL or native)

Run the following command:

```sh
chmod +x build-and-install.sh
./build-and-install.sh
```

Then add `~/lib/provision` to your path.

# How to use it?

The tool doesn't support all kinds of resources yet. To know those supported, type `provision -h`.

There are two basic ways to invoke provision:

**Through command line**, call `provision` followed by the resource types you want to provision, each with its configuration:

```bash
provision --location westus2 storageaccount --SKU Standard_GRS --name main webapp --settings 'CONNECTION_STRING=$MAIN_STORAGE_ACCOUNT_CONNECTION_STRING'
```

...will generate a storage account named main with SKU Standard_GRS, and a webapp, and set an app setting "CONNECTION_STRING" to "$MAIN_STORAGE_ACCOUNT_CONNECTION_STRING".

The only technicality there is to it is that lists must be space-separated strings (e.g. `--containers "container1 container2"`).


**With a YAML specification file**, which is the preferred way of doing so for demos (since you can iterate on this file). Some samples are provided in the `samples` folder. This command line above corresponds to the following specification:

```yml
deployment:
  resources:
  - type: storageAccount
    sku: Standard_GRS
    name: main
  - type: webapp
    settings:
    - 'CONNECTION_STRING="$MAIN_STORAGE_ACCOUNT_CONNECTION_STRING"'
```

To generate the deployment script, provide the specification file to provision like so: `provision -f deploy.yml`.

# Dependency resolution

The whole point of the provisioning scripts generated by provision is that they're self-sufficient, so it's not possible to specify existing resources.

To specify dependencies between some resources, just use their names. The default name for a resource is "default".

For example, to specify that a function app should use a specific storage account:

```bash
provision functionapp --storage-account funcStorage storageAccount --name funcStorage storageAccount
```

... will generate two storage accounts, one named "default" and one named "funcStorage" ; plus a functionapp, using the "funcStorage" storage account as its storage account.

The order in which you declare the resources doesn't matter, provision outputs resources in a consistent manner.

Provision completes missing dependent resources automatically. In the examples above, a storage account requires a resource-group but none is provided, so Provision will provision one. An app service plan is required by the app service but none is specified, and so one will be provisioned automatically. If a resource has dependencies that are not specified, and a corresponding type is found, then the same resource will be used.

# Why CLI and not ARM templates, Terraform or such tools?

There are things that ARM templates can't do, such as setting default settings, adding service principals, etc. The AZ CLI can do these things, and also deploy templates. So it seems a better default.

It runs within the Azure Shell, which is accessible to everyone, so it seems a pretty good common denominator to build those scripts.

# What do those scripts look like?

The following yml:

```yml
deployment:
  location: westus
  resources:
  - type: storageAccount
  - type: functionApp
```

Generates the following sh:

```bash
#!/bin/bash

PWD=`pwd`
NAME=`basename "$PWD"`
LOCATION="westus"

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
echo "FunctionApp (default)"

DEFAULT_RESOURCE_GROUP="$NAME"
DEFAULT_STORAGE_ACCOUNT="`echo "$STORAGEBASENAME" | sed -e 's/-//g' | sed -E 's/^(.*)$/\L\1/g' | head -c 20`def"
DEFAULT_FUNCTIONAPP="$NAME-`random 5`"
echo "Creating resource group $DEFAULT_RESOURCE_GROUP"
az group create --name $DEFAULT_RESOURCE_GROUP --location $LOCATION --query "properties.provisioningState" -o tsv
echo "Creating storage account $DEFAULT_STORAGE_ACCOUNT"
az storage account create --name $DEFAULT_STORAGE_ACCOUNT --kind StorageV2 --SKU Standard_LRS --location $LOCATION -g $DEFAULT_RESOURCE_GROUP --https-only true --query "provisioningState" -o tsv
DEFAULT_STORAGE_ACCOUNT_CONNECTION_STRING=`az storage account show-connection-string -g $DEFAULT_RESOURCE_GROUP -n $DEFAULT_STORAGE_ACCOUNT --query connectionString -o tsv`

echo "Creating functionapp $DEFAULT_FUNCTIONAPP"
az functionapp create -g $DEFAULT_RESOURCE_GROUP --consumption-plan-location $LOCATION --name $DEFAULT_FUNCTIONAPP --storage-account $DEFAULT_STORAGE_ACCOUNT --query "state" -o tsv



echo "Generating cleanup script"
echo "#!/bin/bash

echo 'Removing resource group $DEFAULT_RESOURCE_GROUP'
az group delete --name $DEFAULT_RESOURCE_GROUP --yes


" > cleanup.sh
chmod +x cleanup.sh
        
echo "    Resource group name: $DEFAULT_RESOURCE_GROUP"
echo "  Storage account (def): $DEFAULT_STORAGE_ACCOUNT"
echo "      Storage key (def): $DEFAULT_STORAGE_ACCOUNT_CONNECTION_STRING"
echo "          Function Name: $DEFAULT_FUNCTIONAPP"
echo "           Function URL: https://$DEFAULT_FUNCTIONAPP_HOSTNAME"
```
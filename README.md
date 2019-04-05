Generate provisioning scripts for use with the Azure CLI

```
provision keyvault --serviceprincipal app
```

Will generate a script that generates a Resource Group, a Service Principal and a KeyVault.

# WHY would I do that?

Because of infrastructure automation for ephemeral artifacts.

When experimenting with things, doing labs, constructing demos, the tendency is to go quick and dirty and create resource directly on the Azure Portal. This approach is not perennial and as such is hardly repeatable. Demos or example built for other parties (e.g. customers) start by having to provision manually resources in a subscription, which is suboptimal.

On the other hand writing provisioning scripts, templates or recipes is a tedious and time-consuming task, which explains why it's not the default behavior.

This is where Provision plays its role: it lowers the friction of creating those script by automating a large portion of it.

# Install

You need .net core 2.1 to build and install it. I suggest installing it in any Linux shell - either the Azure Shell, or a local Ubuntu (WSL or native)

Run the following command:

```sh
chmod +x build-and-install.sh
./build-and-install.sh
```

Then add `~/lib/provision` to your path.

# How to use it?

The tool doesn't support all kinds of resources yet. To know those supported, go to the `src/Resources` folder.


# Why CLI and not ARM templates, Terraform or such tools?

There are things that ARM templates can't do, such as setting default settings, adding service principals, etc. The AZ CLI can do these things, and also deploy templates. So it seems a better default.

It runs within the Azure Shell, which is accessible to everyone, so it seems a pretty good common denominator to build those scripts.
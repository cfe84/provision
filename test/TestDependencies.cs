using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using Xunit;

namespace Provision.Test
{
    public class TestDependencies
    {
        [Fact]
        public void Injector_should_inject()
        {
            // prepare
            var context = new Provision.Context();
            // since we're relying on a lot of Reflection trickery, we
            // can't inject anymore. This is very sad, and maybe we should
            // do otherwise 
            var dependedUpon = new ResourceGroup(context);
            var dependant = new StorageAccount(context);
            context.Resources.Add(dependedUpon);
            context.Resources.Add(dependant);
            // execute
            new Injector(context).Inject();

            // assess
            Assert.Equal(dependedUpon, dependant.ResourceGroup);
        }

        [Fact]
        public void Injector_should_create_missing_dependencies()
        {
            // prepare
            var context = new Provision.Context();
            var dependant1 = new StorageAccount(context);
            var dependant2 = new StorageAccount(context);
            context.Resources.Add(dependant1);
            context.Resources.Add(dependant2);

            // execute
            new Injector(context).Inject();

            // assess
            Assert.Single(context.Resources, resource => resource.GetType() == typeof(ResourceGroup));
            Assert.NotNull(dependant1.ResourceGroup);
            Assert.Equal(dependant1.ResourceGroup, dependant2.ResourceGroup);
        }
        
        [Fact]
        public void Injector_should_return_existing_dep_as_default() {
            // prepare
            var context = new Provision.Context();
            var resourceGroup = new ResourceGroup(context);
            resourceGroup.Name = "a non default name";
            var dependant = new StorageAccount(context);
            context.Resources.Add(dependant);
            context.Resources.Add(resourceGroup);

            // execute
            new Injector(context).Inject();

            // assess
            Assert.Single(context.Resources, resource => resource.GetType() == typeof(ResourceGroup));
            Assert.Equal(resourceGroup, dependant.ResourceGroup);
        }

        [Fact]
        public void Injector_shouldnt_inject_unconfigured_optional_dependencies() {
            // prepare
            var context = new Provision.Context();
            var keyvault = new KeyVault(context);

            // execute
            new Injector(context).Inject();

            // assess
            Assert.DoesNotContain(context.Resources, resource => resource.GetType() == typeof(ServicePrincipal));
        }
    }
}

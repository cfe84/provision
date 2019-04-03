using System;
using System.Collections.Generic;
using System.Linq;
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
            Injector.Inject(context);

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
            Injector.Inject(context);

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
            Injector.Inject(context);

            // assess
            Assert.Single(context.Resources, resource => resource.GetType() == typeof(ResourceGroup));
            Assert.Equal(resourceGroup, dependant.ResourceGroup);
        }

        [Fact]
        public void Utils_should_return_default_deps() {
            // prepare
            var types = new [] {typeof(StorageAccount), typeof(ResourceGroup)};

            // execute
            var dependencies = DependencyUtils.CreateDefaultDependencyRequirementForType(types);
            
            // assess
            var expected1 = new DependencyRequirement{Name = "StorageAccount", ValueName = "default", Type = typeof(StorageAccount)}; 
            var expected2 = new DependencyRequirement{Name = "ResourceGroup", ValueName = "default", Type = typeof(ResourceGroup)}; 
            Func<DependencyRequirement, DependencyRequirement, bool> comparer = (A, B) => A.Name == B.Name && 
                A.Type == B.Type && 
                A.ValueName == B.ValueName;

            Assert.Equal(2, dependencies.Count());

            Assert.Single(dependencies, (element) => comparer(element, expected1));
            Assert.Single(dependencies, (element) => comparer(element, expected2));
        }


        [Fact]
        public void Utils_should_set_variable_name() {
            // prepare
            var dep1 = new DependencyRequirement{Name = "dep1", Type = typeof(StorageAccount), ValueName = "default"};
            var dep2 = new DependencyRequirement{Name = "dep2", Type = typeof(StorageAccount), ValueName = "default"};
            var dep3 = new DependencyRequirement{Name = "dep1", Type = typeof(ResourceGroup), ValueName = "default"};
            var dep4 = new DependencyRequirement{Name = "Dep4", Type = typeof(ResourceGroup), ValueName = "default"};
            var list = new List<DependencyRequirement>{ dep1, dep2, dep3, dep4 };
            // exec
            DependencyUtils.SetDependencyValueName(list, typeof(StorageAccount), "dep1", "newval");
            DependencyUtils.SetDependencyValueName(list, typeof(ResourceGroup), "DEP4", "newval");
            // assess
            Assert.Equal("newval", dep1.ValueName);
            Assert.Equal("default", dep2.ValueName);
            Assert.Equal("default", dep3.ValueName);    
            Assert.Equal("newval", dep4.ValueName);    
        }
    }
}

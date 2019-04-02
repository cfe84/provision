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
            var dependedUpon = A.Fake<IResource>();
            var valueName = "aSpecificStorageAccount";
            A.CallTo(() => dependedUpon.Name).Returns(valueName);
            A.CallTo(() => dependedUpon.DependencyRequirements).Returns(new DependencyRequirement[0]);
            var dependant = A.Fake<IResource>();
            var variableName = "myDependency";
            A.CallTo(() => dependant.DependencyRequirements).Returns(new DependencyRequirement[]{
                new DependencyRequirement {Name = variableName, Type = dependedUpon.GetType(), ValueName = valueName }
            });
            context.Resources.Add(dependedUpon);
            context.Resources.Add(dependant);

            // execute
            Injector.Inject(context);

            // assess
            A.CallTo(() => dependant.InjectDependency(variableName, dependedUpon)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Injector_should_create_missing_dependencies()
        {
            // prepare
            var context = new Provision.Context();
            var dependant = A.Fake<IResource>();
            var variableName1 = "myDependency1";
            var variableName2 = "myDependency2";
            A.CallTo(() => dependant.DependencyRequirements).Returns(new DependencyRequirement[]{
                new DependencyRequirement {Name = variableName1, Type = typeof(StorageAccount) },
                new DependencyRequirement {Name = variableName2, Type = typeof(StorageAccount) },
            });
            context.Resources.Add(dependant);

            // execute
            Injector.Inject(context);

            // assess
            A.CallTo(() => dependant.InjectDependency(variableName1, A<StorageAccount>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => dependant.InjectDependency(variableName2, A<StorageAccount>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Single(context.Resources, resource => resource.GetType() == typeof(StorageAccount));
        }
        
        [Fact]
        public void Injector_should_return_existing_dep_as_default() {
            // prepare
            var context = new Provision.Context();
            var storageAccount = new StorageAccount(context, name: "a non default name");
            var dependant = A.Fake<IResource>();
            var variableName1 = "myDependency1";
            var variableName2 = "myDependency2";
            A.CallTo(() => dependant.DependencyRequirements).Returns(new DependencyRequirement[]{
                new DependencyRequirement {Name = variableName1, Type = typeof(StorageAccount) },
                new DependencyRequirement {Name = variableName2, Type = typeof(StorageAccount) },
            });
            context.Resources.Add(dependant);
            context.Resources.Add(storageAccount);

            // execute
            Injector.Inject(context);

            // assess
            A.CallTo(() => dependant.InjectDependency(variableName1, storageAccount)).MustHaveHappenedOnceExactly();
            A.CallTo(() => dependant.InjectDependency(variableName2, storageAccount)).MustHaveHappenedOnceExactly();
            Assert.Single(context.Resources, resource => resource.GetType() == typeof(StorageAccount));
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
            var list = new DependencyRequirement[] { dep1, dep2, dep3 };
            // exec
            DependencyUtils.SetDependencyValueName(list, typeof(StorageAccount), "dep1", "newval");
            // assess
            Assert.Equal("newval", dep1.ValueName);
            Assert.Equal("default", dep2.ValueName);
            Assert.Equal("default", dep3.ValueName);    
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Provision {
    internal static class DependencyUtils {
        public static IEnumerable<DependencyRequirement> CreateDefaultDependencyRequirementForType(Type[] types) {
            return types.Select(type => 
                new DependencyRequirement {Name = type.Name, Type = type }
            );
        }

        public static void SetDependencyValueName(IEnumerable<DependencyRequirement> dependencies, Type type, string name, string valueName) {
            var matching = dependencies
                .Where(dependency => dependency.Name == name && dependency.Type == type);
            if (matching.Count() != 1) {
                throw new Exception($"No matching dependency {type}:{name}");
            }
            matching.FirstOrDefault().ValueName = valueName;
        }
    }
}
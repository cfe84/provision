using System;
using System.Collections.Generic;
using System.Linq;

namespace Provision {
    internal static class DependencyUtils {
        public static List<DependencyRequirement> CreateDefaultDependencyRequirementForType(Type[] types) {
            return types.Select(type => 
                new DependencyRequirement {Name = type.Name, Type = type }
            ).ToList();
        }

        public static void SetDependencyValueName(List<DependencyRequirement> dependencies, Type type, string name, string valueName) {
            var matching = dependencies
                .Where(dependency => 
                    dependency.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                    && dependency.Type == type);
            if (matching.Count() != 1) {
                throw new Exception($"No matching dependency {type}:{name}");
            }
            var match = matching.First();
            match.ValueName = valueName;
        }
    }
}
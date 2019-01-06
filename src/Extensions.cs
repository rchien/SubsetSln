using System.Collections.Generic;
using System.Linq;
using NuGet.ProjectModel;

namespace SubsetSln
{
    public static class Extensions
    {
        public static IEnumerable<string> GetDependencyProjectUniqueNames(this PackageSpec spec)
        {
            if (spec?.RestoreMetadata?.TargetFrameworks?.FirstOrDefault()?.ProjectReferences == null)
                return Enumerable.Empty<string>();

            return spec.RestoreMetadata.TargetFrameworks.First().ProjectReferences
                .Select(r => r.ProjectUniqueName);
        }

        public static string GetProjectUniqueName(this PackageSpec spec)
        {
            return spec?.RestoreMetadata?.ProjectUniqueName;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using NuGet.ProjectModel;

namespace SubsetSln
{
    class Program
    {
        static void Main(string[] args)
        {
            // Replace to point to your project or solution
            var projectPath = @"C:\Dev\Mojio\platform-sf\src\Mojio.Platform.sln";
            var projNames = new[] {"Mojio.Platform.Gateway.Application" /*, "Mojio.Platform.Ingress.Application"*/};

            var depGraph = DependencyGraphUtility.GenerateDependencyGraph(projectPath);

            var allProjsByName = depGraph.Projects
                .ToDictionary(p => p.Name);

            var allProjsByUniqueName = depGraph.Projects
                .ToDictionary(p => p.GetProjectUniqueName());

            var entryProjs = projNames
                .Select(p => allProjsByName.TryGetValue(p, out var proj) ? proj : null)
                .Where(p => p != null)
                .ToArray();

            // add first level dep of starting projects
            var projDeps = new List<string>();
            projDeps.AddRange(entryProjs.SelectMany(p => p.GetDependencyProjectUniqueNames()).Distinct());

            // depth first traversal
            for (var i = 0; i < projDeps.Count; i++)
            {
                if (!allProjsByUniqueName.TryGetValue(projDeps[i], out var proj)) continue;

                // get dep
                var depNames = proj.GetDependencyProjectUniqueNames().ToArray();
                var depToAppend = depNames.Except(projDeps).ToArray();
                projDeps.AddRange(depToAppend);
            }

            var uniqueProjDeps = projDeps.Distinct()
                .Select(p => allProjsByUniqueName.TryGetValue(p, out var proj) ? proj : null)
                .Where(p => p != null).ToList();

            uniqueProjDeps.AddRange(entryProjs);

            var projPaths = string.Join(' ', uniqueProjDeps.Select(p => p.FilePath));
            var output =
$@"List of dependencies delimited by space:
=====
{projPaths}
=====";

            Console.WriteLine(output);
        }
    }
}
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.ProjectModel;

namespace SubsetSln
{
    public class DependencyGraphUtility
    {
        public static DependencyGraphSpec GenerateDependencyGraph(string projectPath)
        {
            var dotNetRunner = new DotNetRunner();

            var dgOutput = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
                
            string[] arguments = {"msbuild", $"\"{projectPath}\"", "/t:GenerateRestoreGraphFile", $"/p:RestoreGraphOutputPath={dgOutput}"};

            var runStatus = dotNetRunner.Run(Path.GetDirectoryName(projectPath), arguments);

            if (runStatus.IsSuccess)
            {
                var dependencyGraphText = File.ReadAllText(dgOutput);
                return new DependencyGraphSpec(JsonConvert.DeserializeObject<JObject>(dependencyGraphText));
            }

            var errorStr =
$@"Unable to process the the project {projectPath}. Are you sure this is a valid .NET Core or .NET Standard project type?
=====
{runStatus.Output}
=====
";

            throw new Exception(errorStr);
        }
    }
}
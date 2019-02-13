using CommandLine;

namespace Deployment.Console.Options
{
    [Verb("execute", HelpText = "Executes a script that doesn't deploy Windows")]
    public class NonWindowsDeploymentCmdOptions
    {
        [Option("script", Required = true, HelpText = "Script to execute")]
        public string Script { get; set; }
    }
}
using CommandLine;

namespace Deployer.Raspberry.Console.Options
{
    [Verb("execute", HelpText = "Executes a script that doesn't deploy Windows")]
    public class NonWindowsDeploymentCmdOptions
    {
        [Option("disk", Required = true, HelpText = "The disk number of the SD Card to deploy to")]
        public int DiskNumber { get; set; }

        [Option("script", Required = true, HelpText = "Script to execute")]
        public string Script { get; set; }
    }
}
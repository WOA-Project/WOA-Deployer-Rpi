using CommandLine;

namespace Deployment.Console.Options
{
    [Verb("deploy", HelpText = "Executes a Windows deployment script")]
    public class WindowsDeploymentCmdOptions
    {
        [Option("disk", Required = true, HelpText = "The disk number of the SD Card to deploy to")]
        public int DiskNumber { get; set; }

        [Option("wim", Required = true, HelpText = "Windows Image (.wim) to deploy")]
        public string WimImage { get; set; }

        [Option("index", Default = 1, HelpText = "Index of the image to deploy")]
        public int Index { get; set; }

        [Option("compact", Default = false, HelpText = "Enable Compact deployment. Slower, but saves phone disk space")]
        public bool UseCompact { get; set; }
    }
}
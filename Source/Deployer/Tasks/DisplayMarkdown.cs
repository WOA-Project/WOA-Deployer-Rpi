using System.IO;
using System.Threading.Tasks;
using Deployer.Execution;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Displaying Markdown document from {0}")]
    public class DisplayMarkdown : IDeploymentTask
    {
        private readonly string path;
        private readonly IMarkdownDisplayer markdownDisplayer;

        public DisplayMarkdown(string path, IMarkdownDisplayer markdownDisplayer)
        {
            this.path = path;
            this.markdownDisplayer = markdownDisplayer;
        }

        public Task Execute()
        {
            Log.Verbose("Displaying markdown from file {Path}", path);
            return markdownDisplayer.Display("Info", File.ReadAllText(path));
        }
    }
}
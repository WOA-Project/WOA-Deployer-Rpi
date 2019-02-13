using System.Threading.Tasks;
using Deployer.Execution;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Displaying Markdown document")]
    public class DisplayMarkdownMessage : IDeploymentTask
    {
        private readonly string message;

        public DisplayMarkdownMessage(string message)
        {
            this.message = message;
        }

        public Task Execute()
        {
            Log.Information(message);
            return Task.CompletedTask;
        }
    }
}
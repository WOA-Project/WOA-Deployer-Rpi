using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tests
{
    public class ParameterizedTask : IDeploymentTask
    {
        private readonly string argument;

        public ParameterizedTask(string argument)
        {
            this.argument = argument;
        }

        public Task Execute()
        {
            return Task.CompletedTask;
        }
    }
}
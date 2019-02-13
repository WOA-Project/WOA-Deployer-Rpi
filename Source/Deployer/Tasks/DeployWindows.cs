using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tasks
{
    [TaskDescription("Deploying Windows")]
    public class DeployWindows : IDeploymentTask
    {
        private readonly IWindowsDeployer windowsDeployer;

        public DeployWindows(IWindowsDeployer windowsDeployer)
        {
            this.windowsDeployer = windowsDeployer;
        }

        public Task Execute()
        {
            return windowsDeployer.Deploy();
        }
    }
}
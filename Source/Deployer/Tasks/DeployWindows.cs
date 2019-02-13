using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tasks
{
    [TaskDescription("Deploying Windows")]
    public class DeployWindows : IDeploymentTask
    {
        private readonly IDevice device;
        private readonly IDisklayoutPreparer preparer;
        private readonly IWindowsDeployer windowsDeployer;

        public DeployWindows(IDevice device, IDisklayoutPreparer preparer, IWindowsDeployer windowsDeployer)
        {
            this.device = device;
            this.preparer = preparer;
            this.windowsDeployer = windowsDeployer;
        }

        public async Task Execute()
        {
            await preparer.Prepare(await device.GetDeviceDisk());
            await windowsDeployer.Deploy();            
        }
    }
}
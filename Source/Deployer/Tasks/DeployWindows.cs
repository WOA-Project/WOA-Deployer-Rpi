using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tasks
{
    [TaskDescription("Deploying Windows")]
    public class DeployWindows : IDeploymentTask
    {
        private readonly IDeviceProvider deviceProvider;
        private readonly IDisklayoutPreparer preparer;
        private readonly IWindowsDeployer windowsDeployer;

        public DeployWindows(IDeviceProvider deviceProvider, IDisklayoutPreparer preparer, IWindowsDeployer windowsDeployer)
        {
            this.deviceProvider = deviceProvider;
            this.preparer = preparer;
            this.windowsDeployer = windowsDeployer;
        }

        public async Task Execute()
        {
            await preparer.Prepare(await deviceProvider.Device.GetDeviceDisk());
            await windowsDeployer.Deploy();
        }
    }
}
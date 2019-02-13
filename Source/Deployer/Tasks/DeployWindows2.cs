using System.Threading.Tasks;
using Deployer.Execution;
using Deployer.Raspberry;

namespace Deployer.Tasks
{
    [TaskDescription("Deploying Windows")]
    public class DeployWindows2 : IDeploymentTask
    {
        private readonly IDeviceProvider deviceProvider;
        private readonly IDisklayoutPreparer preparer;
        private readonly IWindowsDeployer windowsDeployer;

        public DeployWindows2(IDeviceProvider deviceProvider, IDisklayoutPreparer preparer, IWindowsDeployer windowsDeployer)
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
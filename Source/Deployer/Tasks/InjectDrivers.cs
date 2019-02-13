using System.Threading.Tasks;
using Deployer.Execution;
using Deployer.Services;

namespace Deployer.Tasks
{
    [TaskDescription("Injecting drivers")]
    public class InjectDrivers : IDeploymentTask
    {
        private readonly string origin;
        private readonly IDeviceProvider deviceProvider;
        private readonly IWindowsImageService imageService;

        public InjectDrivers(string origin, IDeviceProvider deviceProvider, IWindowsImageService imageService)
        {
            this.origin = origin;
            this.deviceProvider = deviceProvider;
            this.imageService = imageService;
        }

        public async Task Execute()
        {
            var windowsPartition = await deviceProvider.Device.GetWindowsVolume();
            await imageService.InjectDrivers(origin, windowsPartition);
        }
    }
}
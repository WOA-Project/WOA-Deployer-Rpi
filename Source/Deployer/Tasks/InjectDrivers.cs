using System.Threading.Tasks;
using Deployer.Execution;
using Deployer.Services;

namespace Deployer.Tasks
{
    [TaskDescription("Injecting drivers")]
    public class InjectDrivers : IDeploymentTask
    {
        private readonly string origin;
        private readonly IDevice device;
        private readonly IWindowsImageService imageService;

        public InjectDrivers(string origin, IDevice device, IWindowsImageService imageService)
        {
            this.origin = origin;
            this.device = device;
            this.imageService = imageService;
        }

        public async Task Execute()
        {
            var windowsPartition = await device.GetWindowsVolume();
            await imageService.InjectDrivers(origin, windowsPartition);
        }
    }
}
using System;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Deployer.Services;
using Serilog;

namespace Deployer
{
    public class WindowsDeployer : IWindowsDeployer
    {
        private readonly IWindowsOptionsProvider optionsProvider;
        private readonly IDeviceProvider deviceProvider;
        private readonly IWindowsImageService imageService;
        private readonly IBootCreator bootCreator;
        private readonly IObserver<double> progressObserver;
        private IDevice device;

        public WindowsDeployer(IWindowsOptionsProvider optionsProvider, IDeviceProvider deviceProvider, IWindowsImageService imageService, IBootCreator bootCreator, IObserver<double> progressObserver)
        {
            this.optionsProvider = optionsProvider;
            this.deviceProvider = deviceProvider;
            this.imageService = imageService;
            this.bootCreator = bootCreator;
            this.progressObserver = progressObserver;            
        }

        public async Task Deploy()
        {
            Log.Information("Preparing for Windows deployment");
            device = deviceProvider.Device;

            var options = optionsProvider.Options;
            var windowsVolume = await device.GetWindowsVolume();

            Log.Information("Deploying Windows...");
            await imageService.ApplyImage(windowsVolume, options.ImagePath, options.ImageIndex, options.UseCompact, progressObserver);
            await MakeBootable();
        }

        private async Task MakeBootable()
        {
            Log.Verbose("Making Windows installation bootable...");

            var boot = await device.GetBootVolume();
            var windows = await device.GetWindowsVolume();

            await bootCreator.MakeBootable(boot, windows);
            await boot.Partition.SetGptType(PartitionType.Esp);
            var updatedBootVolume = await device.GetBootVolume();

            if (updatedBootVolume != null)
            {
                Log.Verbose("We shouldn't be able to get a reference to the Boot volume.");
                Log.Verbose("Updated Boot Volume: {@Volume}", new { updatedBootVolume.Label, updatedBootVolume.Partition, });
                if (!Equals(updatedBootVolume.Partition.PartitionType, PartitionType.Esp))
                {
                    Log.Warning("The system partition should be {Esp}, but it's {ActualType}", PartitionType.Esp, updatedBootVolume.Partition.PartitionType);
                }
            }            
        }
    }
}
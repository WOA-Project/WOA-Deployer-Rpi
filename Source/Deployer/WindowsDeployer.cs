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
        private readonly IDevice phone;
        private readonly IWindowsImageService imageService;
        private readonly IBootCreator bootCreator;
        private readonly IObserver<double> progressObserver;

        public WindowsDeployer(IWindowsOptionsProvider optionsProvider, IDevice phone, IWindowsImageService imageService, IBootCreator bootCreator, IObserver<double> progressObserver)
        {
            this.optionsProvider = optionsProvider;
            this.phone = phone;
            this.imageService = imageService;
            this.bootCreator = bootCreator;
            this.progressObserver = progressObserver;
        }

        public async Task Deploy()
        {
            var options = optionsProvider.Options;
            await imageService.ApplyImage(await phone.GetWindowsVolume(), options.ImagePath, options.ImageIndex, options.UseCompact, progressObserver);
            await MakeBootable();
        }

        private async Task MakeBootable()
        {
            Log.Verbose("Making Windows installation bootable...");

            var boot = await phone.GetBootVolume();
            var windows = await phone.GetWindowsVolume();

            await bootCreator.MakeBootable(boot, windows);
            await boot.Partition.SetGptType(PartitionType.Esp);
            var updatedBootVolume = await phone.GetBootVolume();

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
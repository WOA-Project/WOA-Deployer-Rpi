using System;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Deployer.Services;
using Deployer.Tasks;
using Serilog;

namespace Deployer.Raspberry
{
    public class RaspberryDisklayoutPreparer : IDiskLayoutPreparer
    {
        private readonly IImageFlasher imageFlasher;
        private readonly IObserver<double> progressObserver;

        private const string WindowsPartitonLabel = "WindowsARM";

        public RaspberryDisklayoutPreparer(IImageFlasher imageFlasher, IObserver<double> progressObserver)
        {
            this.imageFlasher = imageFlasher;
            this.progressObserver = progressObserver;
        }

        public async Task Prepare(Disk disk)
        {
            Log.Information("Flashing GPT image...");
            await imageFlasher.Flash(disk, @"Core\gpt.zip", progressObserver);
            Log.Information("GPT image flashed");
            
            await CreateWindowsPartition(disk);
        }

        private async Task CreateWindowsPartition(Disk disk)
        {
            Log.Verbose("Creating Windows partition...");

            var windowsPartition = await disk.CreatePartition(ulong.MaxValue);
            var winVolume = await windowsPartition.GetVolume();
            await winVolume.Mount();
            await winVolume.Format(FileSystemFormat.Ntfs, WindowsPartitonLabel);

            Log.Verbose("Windows Partition created successfully");            
        }
    }
}
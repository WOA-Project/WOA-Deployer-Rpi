using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Exceptions;
using Deployer.FileSystem;
using Deployer.Services;
using Deployer.Tasks;
using Serilog;

namespace Deployer.Lumia
{
    public class LumiaDisklayoutPreparer : IDisklayoutPreparer
    {
        private readonly IWindowsOptionsProvider optionsProvider;
        private readonly IEnumerable<ISpaceAllocator<IPhone>> spaceAllocators;
        private readonly IPhone phone;
        private static readonly ByteSize ReservedPartitionSize = ByteSize.FromMegaBytes(200);
        private static readonly ByteSize BootPartitionSize = ByteSize.FromMegaBytes(100);
        private const string BootPartitionLabel = "BOOT";
        private const string WindowsPartitonLabel = "WindowsARM";

        public LumiaDisklayoutPreparer(IWindowsOptionsProvider optionsProvider,  IEnumerable<ISpaceAllocator<IPhone>> spaceAllocators, IPhone phone)
        {
            this.optionsProvider = optionsProvider;
            this.spaceAllocators = spaceAllocators;
            this.phone = phone;
        }

        public async Task Prepare(Disk disk)
        {
            Log.Information("Preparing partitions for Windows deployment...");

            await phone.RemoveExistingWindowsPartitions();
            var options = optionsProvider.Options;
            await AllocateSpace(options.SizeReservedForWindows);
            await CreatePartitions();

            Log.Information("Partition layout ready");
        }

        private async Task AllocateSpace(ByteSize requiredSize)
        {
            Log.Verbose("Verifying the available space...");

            Log.Verbose("We will need {Size} of free space for Windows", requiredSize);

            var hasEnoughSpace = await phone.IsThereEnoughSpace(requiredSize);
            if (!hasEnoughSpace)
            {
                Log.Verbose("There's not enough space in the phone. We will try to allocate it automatically");

                var success = await spaceAllocators.ToObservable()
                    .Select(x => Observable.FromAsync(() => x.TryAllocate(phone, requiredSize)))
                    .Merge(1)
                    .Any(successful => successful);

                if (!success)
                {
                    Log.Verbose("Allocation attempt failed");
                    throw new NotEnoughSpaceException($"Could not allocate {requiredSize} on the phone. Please, try to allocate the necessary space manually and retry.");
                }
                
                Log.Verbose("Space allocated correctly");
            }
            else
            {
                Log.Verbose("We have enough available space to deploy Windows");
            }
        }

        private async Task<WindowsVolumes> CreatePartitions()
        {
            Log.Verbose("Creating Windows partitions...");

            await (await phone.GetDeviceDisk()).CreateReservedPartition((ulong)ReservedPartitionSize.Bytes);

            var bootPartition = await (await phone.GetDeviceDisk()).CreatePartition((ulong)BootPartitionSize.Bytes);
            var bootVolume = await bootPartition.GetVolume();
            await bootVolume.Mount();
            await bootVolume.Format(FileSystemFormat.Fat32, BootPartitionLabel);

            var windowsPartition = await (await phone.GetDeviceDisk()).CreatePartition(ulong.MaxValue);
            var winVolume = await windowsPartition.GetVolume();
            await winVolume.Mount();
            await winVolume.Format(FileSystemFormat.Ntfs, WindowsPartitonLabel);

            Log.Verbose("Windows Partitions created successfully");

            return new WindowsVolumes(await phone.GetBootVolume(), await phone.GetWindowsVolume());
        }
    }
}
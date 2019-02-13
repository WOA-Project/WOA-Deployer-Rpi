using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Exceptions;
using Deployer.FileSystem;
using Deployer.Services;
using Serilog;

namespace Deployer.Raspberry
{
    public class WindowsDeployer : IWindowsDeployer
    {
        private readonly IWindowsOptionsProvider optionsProvider;
        private readonly IPhone phone;
        private readonly IWindowsImageService imageService;
        private readonly IBootCreator bootCreator;
        private readonly IEnumerable<ISpaceAllocator> spaceAllocators;
        private readonly IObserver<double> progressObserver;

        private static readonly ByteSize ReservedPartitionSize = ByteSize.FromMegaBytes(200);
        private static readonly ByteSize BootPartitionSize = ByteSize.FromMegaBytes(100);
        private const string BootPartitionLabel = "BOOT";
        private const string WindowsPartitonLabel = "WindowsARM";

        public WindowsDeployer(IWindowsOptionsProvider optionsProvider, IPhone phone, IWindowsImageService imageService, IBootCreator bootCreator, IEnumerable<ISpaceAllocator> spaceAllocators,  IObserver<double> progressObserver)
        {
            this.optionsProvider = optionsProvider;
            this.phone = phone;
            this.imageService = imageService;
            this.bootCreator = bootCreator;
            this.spaceAllocators = spaceAllocators;
            this.progressObserver = progressObserver;
        }

        public async Task Deploy()
        {
            await phone.RemoveExistingWindowsPartitions();
            var options = optionsProvider.Options;
            await AllocateSpace(options.SizeReservedForWindows);
            var partitions = await CreatePartitions();
            await imageService.ApplyImage(await phone.GetWindowsVolume(), options.ImagePath, options.ImageIndex, options.UseCompact, progressObserver);
            await MakeBootable(partitions);
        }

        private async Task MakeBootable(WindowsVolumes volumes)
        {
            Log.Verbose("Making Windows installation bootable...");

            await bootCreator.MakeBootable(volumes.Boot, volumes.Windows);
            await volumes.Boot.Partition.SetGptType(PartitionType.Esp);
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
    }
}
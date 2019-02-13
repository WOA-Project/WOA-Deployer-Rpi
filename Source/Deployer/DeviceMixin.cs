using System;
using System.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.FileSystem;
using Serilog;

namespace Deployer
{
    public static class DeviceMixin
    {
        private static readonly ByteSize ValidResizeThreshold = ByteSize.FromMegaBytes(200);

        public static async Task EnsureBootPartitionIs(this IDevice device, PartitionType partitionType)
        {
            Partition partition = await GetBootPartition(device);
            if (partition == null)
            {
                partition = (await device.GetBootVolume())?.Partition;
            }

            if (partition == null)
            {
                throw new InvalidOperationException("Cannot get the boot partition");
            }

            await partition.SetGptType(partitionType);            
        }

        public static async Task<Partition> GetBootPartition(this IDevice device)
        {
            var partitions = await (await device.GetDeviceDisk()).GetPartitions();
            var bootPartition = partitions.FirstOrDefault(x => Equals(x.PartitionType, PartitionType.Esp));
            if (bootPartition != null)
            {
                return bootPartition;
            }

            var bootVolume = await device.GetBootVolume();
            return bootVolume?.Partition;
        }

        public static async Task<bool> IsThereEnoughSpace(this IDevice phone, ByteSize requiredSize)
        {
            var disk = await phone.GetDeviceDisk();
            var diff = disk.AvailableSize - requiredSize;
            var isThereEnoughSpace = Math.Abs(diff.MegaBytes) <= ValidResizeThreshold.MegaBytes;

            Log.Verbose("Available - Required => {Available} - {Required} = {Difference}",disk.AvailableSize, requiredSize, diff);
            Log.Verbose("Enough space? {Result}", isThereEnoughSpace);

            return isThereEnoughSpace;
        }
    }
}
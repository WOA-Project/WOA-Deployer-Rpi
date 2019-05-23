using System;
using System.Linq;
using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer.Raspberry
{
    public class RaspberryPi : Device
    {
        private readonly IDisk disk;

        public RaspberryPi(IDisk disk)
        {
            this.disk = disk;
        }

        public override Task<IDisk> GetDeviceDisk()
        {
            if (disk.IsOffline)
            {
                throw new ApplicationException(
                    "The phone disk is offline. Please, set it online with Disk Management or DISKPART.");
            }

            return Task.FromResult(disk);
        }

        public override async Task<IPartition> GetWindowsPartition()
        {
            var winPart = await disk.GetPartitionByVolumeLabel(PartitionLabels.Windows);
            if (winPart.Root == null)
            {
                await winPart.EnsureWritable();
            }

            return winPart;
        }

        protected override Task<bool> IsWoAPresent()
        {
            throw new NotImplementedException();
        }

        public override async Task<IPartition> GetSystemPartition()
        {
            var partitions = await disk.GetPartitions();

            var systemPartition = partitions.First();
            if (systemPartition.Root == null)
            {
                await systemPartition.AssignDriveLetter();
            }

            return systemPartition;
        }
    }
}
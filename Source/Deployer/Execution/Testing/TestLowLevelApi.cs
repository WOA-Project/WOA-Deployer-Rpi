using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.FileSystem;

namespace Deployer.Execution.Testing
{
    public class TestLowLevelApi :  ILowLevelApi
    {
        public Task ResizePartition(Partition partition, ByteSize sizeInBytes)
        {
            throw new NotImplementedException();
        }

        public Task<List<Partition>> GetPartitions(Disk disk)
        {
            throw new NotImplementedException();
        }

        public Task<Volume> GetVolume(Partition partition)
        {
            throw new NotImplementedException();
        }

        public Task<Partition> CreateReservedPartition(Disk disk, ulong sizeInBytes)
        {
            throw new NotImplementedException();
        }

        public Task<Partition> CreatePartition(Disk disk, ulong sizeInBytes)
        {
            throw new NotImplementedException();
        }

        public Task SetPartitionType(Partition partition, PartitionType partitionType)
        {
            throw new NotImplementedException();
        }

        public Task Format(Volume volume, FileSystemFormat ntfs, string fileSystemLabel)
        {
            throw new NotImplementedException();
        }

        public char GetFreeDriveLetter()
        {
            throw new NotImplementedException();
        }

        public Task AssignDriveLetter(Volume volume, char letter)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Volume>> GetVolumes(Disk disk)
        {
            IList<Volume> result = new List<Volume>
            {
                new Volume(new Partition(disk)
                {
                    Letter = 'M',
                    PartitionType = PartitionType.Basic
                })
                {
                    Label = "MainOS",
                    Letter = 'M'
                },
                new Volume(new Partition(disk)
                {
                    Letter = 'E',
                    PartitionType = PartitionType.Esp
                })
                {
                    Label = "EFIESP",
                    Letter = 'E',
                },
                new Volume(new Partition(disk)
                {
                    Letter = 'W',
                    PartitionType = PartitionType.Basic
                })
                {
                    Label = "WindowsARM",
                    Letter = 'W',
                }
            };

            return Task.FromResult(result);
        }

        public Task RemovePartition(Partition partition)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Disk>> GetDisks()
        {
            ICollection<Disk> result = new List<Disk>
            {
                new Disk(this, new DiskInfo
                {
                    FriendlyName = "EFIESP",
                    Size = ByteSize.FromGigaBytes(29)
                })
            };

            return Task.FromResult(result);
        }

        public Task<ICollection<DriverMetadata>> GetDrivers(string path)
        {
            throw new NotImplementedException();
        }

        public Task UpdateStorageCache()
        {
            throw new NotImplementedException();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ByteSizeLib;

namespace Deployer.FileSystem
{
    public class Disk
    {
        public ILowLevelApi LowLevelApi { get; }
        public uint Number { get; }
        public ByteSize Size { get; }
        public ByteSize AllocatedSize { get; }
        public ByteSize AvailableSize => Size - AllocatedSize;

        public Disk(ILowLevelApi lowLevelApi, DiskInfo diskProps)
        {
            LowLevelApi = lowLevelApi;
            FriendlyName = diskProps.FriendlyName;
            Number = diskProps.Number;
            Size = diskProps.Size;
            AllocatedSize = diskProps.AllocatedSize;
            FriendlyName = diskProps.FriendlyName;
            IsSystem = diskProps.IsSystem;
            IsBoot = diskProps.IsBoot;
            IsReadOnly= diskProps.IsReadOnly;
            IsOffline = diskProps.IsOffline;
        }

        public bool IsSystem { get; }

        public bool IsBoot { get; }

        public bool IsReadOnly { get; }

        public bool IsOffline { get; }

        public string FriendlyName { get; }

        public async Task<IList<Volume>> GetVolumes()
        {
            var volumes = await LowLevelApi.GetVolumes(this);
            return volumes;
        }

        public Task<List<Partition>> GetPartitions()
        {
            return LowLevelApi.GetPartitions(this);
        }

        public Task<Partition> CreatePartition(ulong sizeInBytes)
        {
            return LowLevelApi.CreatePartition(this, sizeInBytes);
        }

        public Task<Partition> CreateReservedPartition(ulong sizeInBytes)
        {
            return LowLevelApi.CreateReservedPartition(this, sizeInBytes);
        }

        public async Task<Partition> GetReservedPartition()
        {
            var parts = await LowLevelApi.GetPartitions(this);
            return parts.FirstOrDefault(x => Equals(x.PartitionType, PartitionType.Reserved));
        }

        public async Task<Partition> GetBootEfiEspPartition()
        {
            var parts = await LowLevelApi.GetPartitions(this);
            return parts
                .OrderByDescending(x => x.Number)
                .FirstOrDefault(x => Equals(x.PartitionType, PartitionType.Esp));
        }

        public override string ToString()
        {
            return $"{nameof(Number)}: {Number}, {nameof(Size)}: {Size.ToString()}, {nameof(AllocatedSize)}: {AllocatedSize.ToString()}";
        }
    }
}
using System.Threading.Tasks;
using ByteSizeLib;
using Serilog;

namespace Deployer.FileSystem
{
    public class Partition
    {
        public Partition(Disk disk)
        {
            Disk = disk;
        }

        public Disk Disk { get; private set; }
        public uint Number { get; set; }
        public string Id { get; set; }
        public char? Letter { get; set; }
        public PartitionType PartitionType { get; set; }
        public ILowLevelApi LowLevelApi => Disk.LowLevelApi;

        public override string ToString()
        {
            return $"{nameof(Disk)}: {Disk}, {nameof(Number)}: {Number}";
        }

        public async Task Resize(ByteSize sizeInBytes)
        {
            await LowLevelApi.ResizePartition(this, sizeInBytes);
        }

        public Task<Volume> GetVolume()
        {
            return LowLevelApi.GetVolume(this);
        }

        public async Task SetGptType(PartitionType partitionType)
        {
            Log.Verbose("Setting partition type to {Partition} from {OldType} to {NewType}", this, PartitionType, partitionType);
            await LowLevelApi.SetPartitionType(this, partitionType);
            Log.Verbose("Partition type set");
        }

        public Task Remove()
        {
            return LowLevelApi.RemovePartition(this);
        }
    }
}
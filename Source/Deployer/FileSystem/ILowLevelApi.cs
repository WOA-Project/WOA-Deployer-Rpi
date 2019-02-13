using System.Collections.Generic;
using System.Threading.Tasks;
using ByteSizeLib;

namespace Deployer.FileSystem
{
    public interface ILowLevelApi
    {
        Task ResizePartition(Partition partition, ByteSize sizeInBytes);
        Task<List<Partition>> GetPartitions(Disk disk);
        Task<Volume> GetVolume(Partition partition);
        Task<Partition> CreateReservedPartition(Disk disk, ulong sizeInBytes);
        Task<Partition> CreatePartition(Disk disk, ulong sizeInBytes);
        Task SetPartitionType(Partition partition, PartitionType partitionType);
        Task Format(Volume volume, FileSystemFormat ntfs, string fileSystemLabel);
        char GetFreeDriveLetter();
        Task AssignDriveLetter(Volume volume, char letter);
        Task<IList<Volume>> GetVolumes(Disk disk);
        Task RemovePartition(Partition partition);
        Task<ICollection<Disk>> GetDisks();
        Task<ICollection<DriverMetadata>> GetDrivers(string path);
        Task UpdateStorageCache();
    }    
}
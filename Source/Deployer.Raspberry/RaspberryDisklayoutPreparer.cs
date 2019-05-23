using System.Linq;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Deployer.Tasks;
using Serilog;

namespace Deployer.Raspberry
{
    public class RaspberryDisklayoutPreparer : IDiskLayoutPreparer
    {
        public async Task Prepare(IDisk disk)
        {
            Log.Verbose("Preparing Micro SD...");

            var windowsPartition = await disk.CreatePartition(PartitionType.Basic, "Windows");
            var vol = await windowsPartition.GetVolume();
            await vol.Format(FileSystemFormat.Ntfs, PartitionLabels.Windows);

            var volumes = await disk.GetVolumes();
            var system = volumes.First(x => x.Label == PartitionLabels.System);
            await system.Partition.SetGptType(PartitionType.Esp);

            Log.Verbose("Micro SD ready");
        }
    }
}
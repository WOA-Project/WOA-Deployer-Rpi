using System.Linq;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Serilog;

namespace Deployer.Raspberry
{
    public class RaspberryPi : Device
    {
        private readonly int diskNumber;
        private readonly Disk disk;

        public RaspberryPi(ILowLevelApi lowLevelApi, int diskNumber) : base(lowLevelApi)
        {
            this.diskNumber = diskNumber;
        }

        public RaspberryPi(ILowLevelApi lowLevelApi, Disk disk) : base(lowLevelApi)
        {
            this.disk = disk;
        }

        public override async Task<Disk> GetDeviceDisk()
        {
            if (disk == null)
            {
                var disks = await LowLevelApi.GetDisks();
                return disks.First(x => x.Number == diskNumber);
            }

            return disk;
        }      

        public override async Task<Volume> GetBootVolume()
        {
            return await GetVolume("EFIESP");
        }

        public override async Task RemoveExistingWindowsPartitions()
        {
            Log.Verbose("Cleanup of possible previous Windows 10 ARM64 installation...");

            await RemovePartition("Reserved", await (await GetDeviceDisk()).GetReservedPartition());
            await RemovePartition("WoA ESP", await this.GetBootPartition());
            var winVol = await GetWindowsVolume();
            await RemovePartition("WoA", winVol?.Partition);
        }
    }
}
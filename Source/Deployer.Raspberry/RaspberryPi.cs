using System;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Serilog;

namespace Deployer.Raspberry
{
    public class RaspberryPi : Device
    {
        private readonly Disk disk;

        public RaspberryPi(ILowLevelApi lowLevelApi, Disk disk) : base(lowLevelApi)
        {
            this.disk = disk;
        }

        public override Task<Disk> GetDeviceDisk()
        {
            return Task.FromResult(disk);
        }      

        public override async Task<Volume> GetBootVolume()
        {
            return await GetVolume("BOOT");
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
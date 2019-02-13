using System;
using System.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Exceptions;
using Deployer.FileSystem;
using Deployer.Services;
using Serilog;

namespace Deployer.Lumia
{
    public class Phone : Device, IPhone
    {
        private readonly IPhoneModelReader phoneModelReader;
        private readonly BcdInvokerFactory bcdInvokerFactory;
        protected const string MainOsLabel = "MainOS";
        private static readonly ByteSize MinimumPhoneDiskSize = ByteSize.FromGigaBytes(28);
        private static readonly ByteSize MaximumPhoneDiskSize = ByteSize.FromGigaBytes(34);
        
        private static readonly Guid WinPhoneBcdGuid = Guid.Parse("7619dcc9-fafe-11d9-b411-000476eba25f");
        private Volume efiEspVolume;
        private IBcdInvoker bcdInvoker;

        public Phone(ILowLevelApi lowLevelApi, IPhoneModelReader phoneModelReader, BcdInvokerFactory bcdInvokerFactory) : base(lowLevelApi)
        {
            this.phoneModelReader = phoneModelReader;
            this.bcdInvokerFactory = bcdInvokerFactory;
        }

        public async Task<Volume> GetEfiespVolume()
        {
            return efiEspVolume ?? (efiEspVolume = await GetVolume("EFIESP"));
        }

        public async Task<IBcdInvoker> GetBcdInvoker()
        {
            return bcdInvoker ?? (bcdInvoker = bcdInvokerFactory.Create((await GetEfiespVolume()).GetBcdFullFilename()));
        }

        public async Task<PhoneModel> GetModel()
        {
            return phoneModelReader.GetPhoneModel((await GetDeviceDisk()).Number);
        }

        public async Task<DualBootStatus> GetDualBootStatus()
        {
            Log.Verbose("Getting Dual Boot Status...");

            var isWoaPresent = await IsWoAPresent();
            var isWPhonePresent = await IsWindowsPhonePresent();
            var isOobeFinished = await IsOobeFinished();
            var isBcdEntryPresent = await GetIsEntryPresent(WinPhoneBcdGuid);

            var bootPartition = await this.GetBootPartition();
            
            var isEnabled = bootPartition != null && Equals(bootPartition.PartitionType, PartitionType.Basic) && isBcdEntryPresent;

            var isCapable = isWoaPresent && isWPhonePresent && isOobeFinished;
            var status = new DualBootStatus(isCapable, isEnabled);

            Log.Verbose("WoA Present: {Value}", isWoaPresent);
            Log.Verbose("Windows 10 Mobile Present: {Value}", isWPhonePresent);
            Log.Verbose("OOBE Finished: {Value}", isOobeFinished);

            Log.Verbose("Dual Boot Status retrieved");
            Log.Verbose("Dual Boot Status is {@Status}", status);

            return status;
        }

        private async Task<bool> GetIsEntryPresent(Guid guid)
        {
            var invoker = await GetBcdInvoker();
            var result = invoker.Invoke();
            return result.Contains(guid.ToString());
        }

        public async Task EnableDualBoot(bool enable)
        {
            var status = await GetDualBootStatus();
            if (!status.CanDualBoot)
            {
                throw new InvalidOperationException("Cannot enable Dual Boot");
            }

            if (status.IsEnabled != enable)
            {
                if (enable)
                {
                    await EnableDualBoot();
                }
                else
                {
                    await DisableDualBoot();
                }
            }
            else
            {
                Log.Debug("Dual Boot status will not change");
            }
        }

        private async Task EnableDualBoot()
        {
            Log.Verbose("Enabling Dual Boot...");

            await this.EnsureBootPartitionIs(PartitionType.Basic);

            var volume = await GetEfiespVolume();
            var bcdInvoker = new BcdInvoker(volume.GetBcdFullFilename());
            bcdInvoker.Invoke($@"/set {{{WinPhoneBcdGuid}}} description ""Windows 10 Phone""");
            bcdInvoker.Invoke($@"/displayorder {{{WinPhoneBcdGuid}}} /addfirst");
            bcdInvoker.Invoke($@"/default {{{WinPhoneBcdGuid}}}");
            
            Log.Verbose("Dual Boot enabled");
        }

        private async Task DisableDualBoot()
        {
            Log.Verbose("Disabling Dual Boot...");

            await this.EnsureBootPartitionIs(PartitionType.Esp);

            var bcdInvoker = new BcdInvoker((await GetEfiespVolume()).GetBcdFullFilename());
            bcdInvoker.Invoke($@"/displayorder {{{WinPhoneBcdGuid}}} /remove");

            Log.Verbose("Dual Boot disabled");
        }

        public Task<Volume> GetDataVolume()
        {
            return GetVolume("Data");
        }

        public override async Task RemoveExistingWindowsPartitions()
        {
            Log.Verbose("Cleanup of possible previous Windows 10 ARM64 installation...");

            await RemovePartition("Reserved", await (await GetDeviceDisk()).GetReservedPartition());
            await RemovePartition("WoA ESP", await DeviceMixin.GetBootPartition(this));
            var winVol = await GetWindowsVolume();
            await RemovePartition("WoA", winVol?.Partition);
        }

        public override async Task<Disk> GetDeviceDisk()
        {
            var disks = await LowLevelApi.GetDisks();
            foreach (var disk in disks.Where(x => x.Number != 0))
            {
                var hasCorrectSize = HasCorrectSize(disk);

                if (hasCorrectSize)
                {
                    var volumes = await disk.GetVolumes();
                    var mainOsVol = volumes.FirstOrDefault(x => x.Label == MainOsLabel);
                    if (mainOsVol != null)
                    {
                        return disk;
                    }
                }
            }

            throw new PhoneDiskNotFoundException("Cannot get the Phone Disk. Please, verify that the Phone is in Mass Storage Mode.");
        }

        private static bool HasCorrectSize(Disk disk)
        {
            var moreThanMinimum = disk.Size > MinimumPhoneDiskSize;
            var lessThanMaximum = disk.Size < MaximumPhoneDiskSize;
            return moreThanMinimum && lessThanMaximum;
        }

        public override async Task<Volume> GetBootVolume()
        {
            return await GetVolume("BOOT");
        }
    }
}

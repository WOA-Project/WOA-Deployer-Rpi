using System.Linq;
using System.Threading.Tasks;
using Deployer.Exceptions;
using Deployer.FileSystem;

namespace Deployer.Lumia.NetFx
{
    public class TestPhone : Phone
    {
        public TestPhone(ILowLevelApi lowLevelApi, IPhoneModelReader phoneModelReader, BcdInvokerFactory bcdInvokerFactory) : base(lowLevelApi, phoneModelReader, bcdInvokerFactory)
        {
        }

        public override async Task<Disk> GetDeviceDisk()
        {
            var disks = await LowLevelApi.GetDisks();
            foreach (var disk in disks.Where(x => x.Number != 0))
            {
                if (true)
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
    }
}
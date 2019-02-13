using System.Threading.Tasks;
using ByteSizeLib;

namespace Deployer.Lumia
{
    public interface ISpaceAllocator<in TDev> where TDev : IDevice
    {
        Task<bool> TryAllocate(TDev phone, ByteSize requiredSpace);
    }
}
using System.Threading.Tasks;
using ByteSizeLib;

namespace Deployer.Raspberry
{
    public interface ISpaceAllocator
    {
        Task<bool> TryAllocate(IPhone phone, ByteSize requiredSpace);
    }
}
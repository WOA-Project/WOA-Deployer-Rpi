using System.Threading.Tasks;
using Deployer.FileSystem;
using Deployer.Services;

namespace Deployer.Lumia
{
    public interface IPhone : IDevice
    {
        Task<Volume> GetEfiespVolume();
        Task<IBcdInvoker> GetBcdInvoker();
        Task<PhoneModel> GetModel();
        Task<DualBootStatus> GetDualBootStatus();
        Task EnableDualBoot(bool enable);
        Task<Volume> GetDataVolume();
    }
}
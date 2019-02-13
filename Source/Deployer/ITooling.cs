using System.Threading.Tasks;

namespace Deployer.Lumia
{
    public interface ITooling
    {
        Task InstallGpu();
        Task ToogleDualBoot(bool isEnabled);
    }
}
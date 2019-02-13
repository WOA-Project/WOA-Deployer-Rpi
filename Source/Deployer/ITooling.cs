using System.Threading.Tasks;

namespace Deployer
{
    public interface ITooling
    {
        Task InstallGpu();
        Task ToogleDualBoot(bool isEnabled);
    }
}
using System.Threading.Tasks;

namespace Deployer.Raspberry
{
    public interface ITooling
    {
        Task InstallGpu();
        Task ToogleDualBoot(bool isEnabled);
    }
}
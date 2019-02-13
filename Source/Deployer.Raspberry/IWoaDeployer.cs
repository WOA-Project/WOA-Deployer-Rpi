using System.Threading.Tasks;

namespace Deployer.Raspberry
{
    public interface IWoaDeployer
    {
        Task Deploy();
        Task ToogleDualBoot(bool p0);
        Task InstallGpu();
    }
}
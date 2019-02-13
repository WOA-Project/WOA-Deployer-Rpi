using System.Threading.Tasks;

namespace Deployer
{
    public interface IWindowsDeployer
    {
        Task Deploy();
    }
}
using System.Threading.Tasks;

namespace Deployer.Raspberry
{
    public interface IWoaDeployer
    {
        Task Deploy();
    }
}
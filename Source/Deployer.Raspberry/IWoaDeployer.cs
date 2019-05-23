using System.Threading.Tasks;
using ByteSizeLib;

namespace Deployer.Raspberry
{
    public interface IWoaDeployer
    {
        Task Deploy();
    }
}
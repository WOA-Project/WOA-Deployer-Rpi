using System.Threading.Tasks;

namespace Deployer
{
    public interface ICoreDeployer<TDevice> where TDevice : Device
    {
        Task<bool> AreDeploymentFilesValid();
        Task Deploy(TDevice phone);
    }
}
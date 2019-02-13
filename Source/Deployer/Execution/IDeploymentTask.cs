using System.Threading.Tasks;

namespace Deployer.Execution
{
    public interface IDeploymentTask
    {
        Task Execute();
    }
}
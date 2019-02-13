using System.Threading.Tasks;
using Serilog;

namespace Deployer.Execution.Testing
{
    public class TestWindowsDeployer : IWindowsDeployer
    {
        public Task Deploy()
        {
            Log.Verbose("Deploying Windows");
            return Task.CompletedTask;
        }
    }
}
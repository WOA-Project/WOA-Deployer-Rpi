using System.Threading.Tasks;

namespace Deployer.Execution
{
    public interface IScriptRunner
    {
        Task Run(Script script);
    }
}
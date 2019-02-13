using System.Threading.Tasks;

namespace Deployer
{
    public interface IPathBuilder
    {
        Task<string> Replace(string str);
    }
}
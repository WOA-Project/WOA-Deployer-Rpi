using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer
{
    public interface IBootCreator
    {
        Task MakeBootable(Volume boot, Volume windows);
    }
}
using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer.Raspberry
{
    public interface IDiskLayoutBuilder
    {
        Task CreateLayout(Disk disk);
    }
}
using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer.Tasks
{
    public interface IDisklayoutPreparer
    {
        Task Prepare(Disk disk);
    }
}
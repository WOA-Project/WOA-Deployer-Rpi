using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tasks
{
    [TaskDescription("Copying folder {0} to {1}")]
    public class CopyDirectory : IDeploymentTask
    {
        private readonly string origin;
        private readonly string destination;
        private readonly IFileSystemOperations fileSystemOperations;

        public CopyDirectory(string origin, string destination, IFileSystemOperations fileSystemOperations)
        {
            this.origin = origin;
            this.destination = destination;
            this.fileSystemOperations = fileSystemOperations;
        }

        public Task Execute()
        {
            return fileSystemOperations.CopyDirectory(origin, destination);
        }
    }
}
using System.IO;

namespace Deployer.Services
{
    public class DeploymentPaths
    {
        private readonly string rootPath;

        public DeploymentPaths(string rootPath)
        {
            this.rootPath = rootPath;
        }
        
        public string PreOobe => Path.Combine(rootPath, "Drivers", "Pre-OOBE");
        public string PostOobe => Path.Combine(rootPath, "Drivers", "Post-OOBE");
        public string BootPatchFolder => Path.Combine(rootPath, "Patch");
    }
}
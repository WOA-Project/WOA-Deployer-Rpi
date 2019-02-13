using System;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Deployer.Services;
using Serilog;

namespace Deployer.Execution.Testing
{
    public class TestWindowsImageService : IWindowsImageService
    {
        public Task ApplyImage(Volume volume, string imagePath, int imageIndex = 1, bool useCompact = false, IObserver<double> progressObserver = null)
        {
            Log.Verbose("Applying Windows Image {Image}{Index} to {Volume}", imagePath, imageIndex, volume.Label);
            return Task.CompletedTask;
        }

        public Task InjectDrivers(string path, Volume volume)
        {
            Log.Verbose("Injecting drivers from {Path} into {Volume}", path, volume.Label);

            return Task.CompletedTask;
        }

        public Task RemoveDriver(string path, Volume volume)
        {
            Log.Verbose("Removing driver {Path} from {Volume}", path, volume);
            
            return Task.CompletedTask;
        }
    }
}
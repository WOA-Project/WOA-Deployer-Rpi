using System;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Deployer.Exceptions;
using Deployer.FileSystem;
using Deployer.Services;
using Deployer.Services.Wim;
using Deployer.Utils;
using Serilog;

namespace Deployer.Filesystem.FullFx
{
    public abstract class ImageServiceBase : IWindowsImageService
    {
        public abstract Task ApplyImage(Volume volume, string imagePath, int imageIndex = 1, bool useCompact = false, 
            IObserver<double> progressObserver = null);

        protected static void EnsureValidParameters(Volume volume, string imagePath, int imageIndex)
        {
            if (volume == null)
            {
                throw new ArgumentNullException(nameof(volume));
            }

            var applyDir = volume.RootDir.Name;

            if (applyDir == null)
            {
                throw new ArgumentException("The volume to apply the image is invalid");
            }

            if (imagePath == null)
            {
                throw new ArgumentNullException(nameof(imagePath));
            }

            EnsureValidImage(imagePath, imageIndex);
        }

        private static void EnsureValidImage(string imagePath, int imageIndex)
        {
            Log.Verbose("Checking image at {Path}, with index {Index}", imagePath, imagePath);

            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException($"Image not found: {imagePath}. Please, verify that the file exists and it's accessible.");
            }

            Log.Verbose("Image file at '{ImagePath}' exists", imagePath);

            using (var stream = File.OpenRead(imagePath))
            {
                var metadata = new WindowsImageMetadataReader().Load(stream);
                var imageMetadata = metadata.Images.Single(x => x.Index == imageIndex);
                if (imageMetadata.Architecture != Architecture.Arm64)
                {
                    throw new InvalidImageException("The selected image isn't for the ARM64 architecture.");
                }
            }            
        }

        public async Task InjectDrivers(string path, Volume volume)
        {
            var outputSubject = new Subject<string>();
            var subscription = outputSubject.Subscribe(Log.Verbose);
            var resultCode = await ProcessUtils.RunProcessAsync(WindowsCommandLineUtils.Dism, $@"/Add-Driver /Image:{volume.RootDir.Name} /Driver:""{path}"" /Recurse /ForceUnsigned", outputSubject, outputSubject);
            subscription.Dispose();
            
            if (resultCode != 0)
            {
                throw new DeploymentException(
                    $"There has been a problem during deployment: DISM exited with code {resultCode}.");
            }
        }

        public async Task RemoveDriver(string path, Volume volume)
        {
            var outputSubject = new Subject<string>();
            var subscription = outputSubject.Subscribe(Log.Verbose);
            var resultCode = await ProcessUtils.RunProcessAsync(WindowsCommandLineUtils.Dism, $@"/Remove-Driver /Image:{volume.RootDir.Name} /Driver:""{path}""", outputSubject, outputSubject);
            subscription.Dispose();
            
            if (resultCode != 0)
            {
                throw new DeploymentException(
                    $"There has been a problem during removal: DISM exited with code {resultCode}.");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Utils;
using Serilog;

namespace Deployer.FileSystem
{
    public class Volume
    {
        private DirectoryInfo rootDir;

        public Volume(Partition partition)
        {
            Partition = partition;
        }

        public string Label { get; set; }
        public ByteSize Size { get; set; }
        public Partition Partition { get; set; }
        public char? Letter { get; set; }

        public DirectoryInfo RootDir => rootDir ?? (rootDir = new DirectoryInfo($"{Letter}:"));

        public Task Format(FileSystemFormat ntfs, string fileSystemLabel)
        {
            return Partition.LowLevelApi.Format(this, ntfs, fileSystemLabel);
        }

        public ILowLevelApi LowLevelApi => Partition.LowLevelApi;

        public async Task Mount()
        {
            Log.Verbose("Mounting volume {Volume}", this);
            var driveLetter = LowLevelApi.GetFreeDriveLetter();
            await LowLevelApi.AssignDriveLetter(this, driveLetter);

            await Observable.Defer(() => Observable.Return(UpdateLetter(driveLetter))).RetryWithBackoffStrategy();
        }

        private Unit UpdateLetter(char driveLetter)
        {
            try
            {
                rootDir = new DirectoryInfo($"{driveLetter}:");
                return Unit.Default;
            }
            catch (Exception)
            {
                Log.Verbose("Cannot get path for drive letter {DriveLetter} while mounting partition {Partition}", driveLetter, this);
                throw;
            }
        }

        public override string ToString()
        {
            return $"{nameof(Label)}: {Label}, {nameof(Size)}: {Size}, {nameof(Partition)}: {Partition}, {nameof(Letter)}: {Letter}";
        }

        public Task<ICollection<DriverMetadata>> GetDrivers()
        {
            if (Partition.Letter == null)
            {
                throw new InvalidOperationException("The partition doesn't have a drive letter");
            }

            return LowLevelApi.GetDrivers(Partition.Letter + ":\\");
        }
    }
}
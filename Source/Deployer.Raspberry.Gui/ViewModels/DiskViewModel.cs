using ByteSizeLib;
using Deployer.FileSystem;

namespace Deployer.Raspberry.Gui.ViewModels
{
    public class DiskViewModel
    {
        private readonly Disk disk;

        public DiskViewModel(Disk disk)
        {
            this.disk = disk;
        }

        public uint Number => disk.Number + 1;
        public string FriendlyName => disk.FriendlyName;
        public ByteSize Size => disk.Size;
        public bool IsUsualTarget => Size > ByteSize.FromGigaBytes(14) && Size < ByteSize.FromGigaBytes(200);
        public Disk Disk => disk;

        public override string ToString()
        {
            return $"Disk number {disk.Number}, {disk.FriendlyName}, Capacity: {disk.Size}";
        }
    }
}
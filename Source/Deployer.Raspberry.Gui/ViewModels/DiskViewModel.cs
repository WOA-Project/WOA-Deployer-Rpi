using ByteSizeLib;
using Deployer.FileSystem;
using Deployer.NetFx;

namespace Deployer.Raspberry.Gui.ViewModels
{
    public class DiskViewModel
    {
        private readonly IDisk disk;

        public DiskViewModel(IDisk disk)
        {
            this.disk = disk;
        }

        public uint Number => disk.Number + 1;
        public string FriendlyName => disk.FriendlyName;
        public ByteSize Size => disk.Size;
        public bool IsUsualTarget => Size > ByteSize.FromGigaBytes(1) && Size < ByteSize.FromGigaBytes(200);
        public IDisk IDisk => disk;

        public override string ToString()
        {
            return $"IDisk number {disk.Number}, {disk.FriendlyName}, Capacity: {disk.Size}";
        }
    }
}
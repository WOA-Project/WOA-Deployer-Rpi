using ByteSizeLib;

namespace Deployer.FileSystem
{
    public class DiskInfo
    {
        public string FriendlyName { get; set; }
        public uint Number { get; set; }
        public ByteSize Size { get; set; }
        public ByteSize AllocatedSize { get; set; }
        public bool IsSystem { get; set; }
        public bool IsBoot { get; set; }
        public bool IsOffline { get; set; }
        public bool IsReadOnly { get; set; }
    }
}
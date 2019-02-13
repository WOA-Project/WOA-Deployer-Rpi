using ByteSizeLib;

namespace Deployer
{
    public class WindowsDeploymentOptions
    {
        public string ImagePath { get; set; }
        public int ImageIndex { get; set; }
        public ByteSize SizeReservedForWindows { get; set; }
        public bool UseCompact { get; set; }
    }
}
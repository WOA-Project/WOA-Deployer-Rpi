using Deployer.FileSystem;

namespace Deployer.Services
{
    public class WindowsVolumes
    {
        public WindowsVolumes(Volume bootVolume, Volume windowsVolume)
        {
            Boot = bootVolume;
            Windows = windowsVolume;
        }

        public Volume Boot { get; }
        public Volume Windows { get; }
    }
}
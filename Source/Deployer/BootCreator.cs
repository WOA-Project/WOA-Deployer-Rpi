using System.IO;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Deployer.Services;
using Deployer.Utils;
using Serilog;

namespace Deployer
{
    public class BootCreator : IBootCreator
    {
        private readonly IBcdInvokerFactory bcdInvokerFactory;

        public BootCreator(IBcdInvokerFactory bcdInvokerFactory)
        {
            this.bcdInvokerFactory = bcdInvokerFactory;
        }

        public async Task MakeBootable(Volume boot, Volume windows)
        {
            Log.Verbose("Making Windows installation bootable...");

            var bcdPath = Path.Combine(boot.RootDir.Name, "EFI", "Microsoft", "Boot", "BCD");
            var bcdInvoker = bcdInvokerFactory.Create(bcdPath);
            var windowsPath = Path.Combine(windows.RootDir.Name, "Windows");
            var bootDriveLetter = boot.Letter;

            await ProcessUtils.RunProcessAsync(WindowsCommandLineUtils.BcdBoot, $@"{windowsPath} /f UEFI /s {bootDriveLetter}:");
            bcdInvoker.Invoke("/set {default} testsigning on");
            bcdInvoker.Invoke("/set {default} recoveryenabled no");
            bcdInvoker.Invoke("/set {default} nointegritychecks on");
        }
    }
}
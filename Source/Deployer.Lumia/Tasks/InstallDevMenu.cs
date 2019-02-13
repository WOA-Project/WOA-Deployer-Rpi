using System.IO;
using System.Threading.Tasks;
using Deployer.Execution;
using Deployer.Utils;

namespace Deployer.Lumia.Tasks
{
    [TaskDescription("Installing Development Menu")]
    public class InstallDevMenu : IDeploymentTask
    {
        private readonly string rootFilesPath;
        private readonly IPhone phone;
        private readonly IBcdInvokerFactory bcdInvokerFactory;
        private readonly IFileSystemOperations fileSystemOperations;

        public InstallDevMenu(string rootFilesPath, IPhone phone, IBcdInvokerFactory bcdInvokerFactory, IFileSystemOperations fileSystemOperations)
        {
            this.rootFilesPath = rootFilesPath;
            this.phone = phone;
            this.bcdInvokerFactory = bcdInvokerFactory;
            this.fileSystemOperations = fileSystemOperations;
        }
        public async Task Execute()
        {
            var efiespVolume = await phone.GetEfiespVolume();
            var rootDir = efiespVolume.RootDir.Name;
            var bcdInvoker = bcdInvokerFactory.Create(Path.Combine(rootDir, "EFI", "Microsoft", "Boot", "BCD"));

            var destination = Path.Combine(rootDir, "Windows", "System32", "BOOT");
            await fileSystemOperations.CopyDirectory(Path.Combine(rootFilesPath), destination);
            var guid = FormattingUtils.GetGuid(bcdInvoker.Invoke(@"/create /d ""Developer Menu"" /application BOOTAPP"));
            bcdInvoker.Invoke($@"/set {{{guid}}} path \Windows\System32\BOOT\developermenu.efi");
            bcdInvoker.Invoke($@"/set {{{guid}}} device partition={rootDir}");
            bcdInvoker.Invoke($@"/set {{{guid}}} testsigning on");
            bcdInvoker.Invoke($@"/set {{{guid}}} nointegritychecks on");
            bcdInvoker.Invoke($@"/displayorder {{{guid}}} /addlast");
        }
    }
}
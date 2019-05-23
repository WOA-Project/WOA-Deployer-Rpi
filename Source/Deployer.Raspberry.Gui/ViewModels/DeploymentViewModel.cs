using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Deployer.NetFx;
using Deployer.Tasks;
using Deployer.UI;
using Deployer.UI.ViewModels;
using Grace.DependencyInjection.Attributes;
using ReactiveUI;
using Serilog;

namespace Deployer.Raspberry.Gui.ViewModels
{
    [Metadata("Name", "Deployment")]
    [Metadata("Order", 0)]
    public class DeploymentViewModel : ReactiveObject, ISection
    {
        private readonly IDeploymentContext context;
        private readonly IWoaDeployer deployer;
        private readonly UIServices uiServices;
        private readonly AdvancedViewModel advancedViewModel;
        private readonly WimPickViewModel wimPickViewModel;
        private DiskViewModel selectedDisk;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IRaspberryPiSettingsService raspberryPiSettingsService;
        private readonly ObservableAsPropertyHelper<bool> isBusyHelper;
        private readonly ObservableAsPropertyHelper<IEnumerable<DiskViewModel>> disks;


        public DeploymentViewModel(
            IDeploymentContext context,
            IOperationContext operationContext,
            IWoaDeployer deployer, UIServices uiServices, AdvancedViewModel advancedViewModel,
            WimPickViewModel wimPickViewModel, IFileSystemOperations fileSystemOperations, IRaspberryPiSettingsService raspberryPiSettingsService,
            IDiskRoot diskRoot)
        {
            this.context = context;
            this.deployer = deployer;
            this.uiServices = uiServices;
            this.advancedViewModel = advancedViewModel;
            this.wimPickViewModel = wimPickViewModel;
            this.fileSystemOperations = fileSystemOperations;
            this.raspberryPiSettingsService = raspberryPiSettingsService;

            var isSelectedWim = wimPickViewModel.WhenAnyObservable(x => x.WimMetadata.SelectedImageObs)
                .Select(metadata => metadata != null);

            FullInstallWrapper = new CommandWrapper<Unit, Unit>(this,
                ReactiveCommand.CreateFromTask(Deploy, isSelectedWim), uiServices.ContextDialog, operationContext);
            IsBusyObservable = FullInstallWrapper.Command.IsExecuting;
            isBusyHelper = IsBusyObservable.ToProperty(this, model => model.IsBusy);

            RefreshDisksCommandWrapper = new CommandWrapper<Unit, IList<IDisk>>(this,
                ReactiveCommand.CreateFromTask(diskRoot.GetDisks), uiServices.ContextDialog, operationContext);
            disks = RefreshDisksCommandWrapper.Command
                .Select(x => x.Select(disk => new DiskViewModel(disk)))
                .ToProperty(this, x => x.Disks);

            this.WhenAnyValue(x => x.SelectedDisk).Where(x => x != null).Subscribe(x => context.Device = new RaspberryPi(x.IDisk));
        }

        public bool IsBusy => isBusyHelper.Value;

        public CommandWrapper<Unit, IList<IDisk>> RefreshDisksCommandWrapper { get; set; }

        public DiskViewModel SelectedDisk
        {
            get => selectedDisk;
            set => this.RaiseAndSetIfChanged(ref selectedDisk, value);
        }


        public IEnumerable<DiskViewModel> Disks => disks.Value;

        private async Task Deploy()
        {
            Log.Information("# Starting deployment...");

            var windowsDeploymentOptions = new WindowsDeploymentOptions
            {
                ImagePath = wimPickViewModel.WimMetadata.Path,
                ImageIndex = wimPickViewModel.WimMetadata.SelectedDiskImage.Index,
                UseCompact = advancedViewModel.UseCompactDeployment,
            };

            context.DeploymentOptions = windowsDeploymentOptions;

            await CleanDownloadedIfNeeded();
            await deployer.Deploy();

            Log.Information("Deployment successful");

            await uiServices.Dialog.PickOptions(Resources.WindowsDeployedSuccessfully, new List<Option>()
            {
                new Option("Close")
            });
        }

        private async Task CleanDownloadedIfNeeded()
        {
            if (!raspberryPiSettingsService.CleanDownloadedBeforeDeployment)
            {
                return;
            }

            if (fileSystemOperations.DirectoryExists(AppPaths.ArtifactDownload))
            {
                Log.Information("Deleting previously downloaded deployment files");
                await fileSystemOperations.DeleteDirectory(AppPaths.ArtifactDownload);
            }
        }

        public CommandWrapper<Unit, Unit> FullInstallWrapper { get; set; }
        public IObservable<bool> IsBusyObservable { get; }
    }
}
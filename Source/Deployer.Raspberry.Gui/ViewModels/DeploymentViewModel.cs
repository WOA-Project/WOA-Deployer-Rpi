using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Deployer.Gui.Common;
using Deployer.Gui.Common.Services;
using ReactiveUI;

namespace Deployer.Raspberry.Gui.ViewModels
{
    public class DeploymentViewModel : ReactiveObject, IBusy
    {
        private readonly IWindowsOptionsProvider optionsProvider;
        private readonly IWoaDeployer deployer;
        private readonly UIServices uiServices;
        private readonly AdvancedViewModel advancedViewModel;
        private readonly WimPickViewModel wimPickViewModel;
        private readonly ObservableAsPropertyHelper<bool> isBusy;
        private DiskViewModel selectedDisk;
        private readonly ObservableAsPropertyHelper<IEnumerable<DiskViewModel>> disks;

        public DeploymentViewModel(IDeviceProvider deviceProvider,
            IWindowsOptionsProvider optionsProvider,
            IWoaDeployer deployer, UIServices uiServices, AdvancedViewModel advancedViewModel,
            WimPickViewModel wimPickViewModel, ILowLevelApi lowLevelApi)
        {
            this.optionsProvider = optionsProvider;
            this.deployer = deployer;
            this.uiServices = uiServices;
            this.advancedViewModel = advancedViewModel;
            this.wimPickViewModel = wimPickViewModel;

            var isSelectedWim = wimPickViewModel.WhenAnyObservable(x => x.WimMetadata.SelectedImageObs)
                .Select(metadata => metadata != null);

            FullInstallWrapper = new CommandWrapper<Unit, Unit>(this,
                ReactiveCommand.CreateFromTask(Deploy, isSelectedWim), uiServices.DialogService);
            IsBusyObservable = FullInstallWrapper.Command.IsExecuting;
            isBusy = IsBusyObservable.ToProperty(this, model => model.IsBusy);

            RefreshDisksCommandWrapper = new CommandWrapper<Unit, ICollection<Disk>>(this,
                ReactiveCommand.CreateFromTask(lowLevelApi.GetDisks), uiServices.DialogService);
            disks = RefreshDisksCommandWrapper.Command
                .Select(x => x
                    .Where(y => !y.IsBoot && !y.IsSystem && !y.IsOffline)
                    .Select(disk => new DiskViewModel(disk)))
                .ToProperty(this, x => x.Disks);

            this.WhenAnyValue(x => x.SelectedDisk).Where(x => x != null).Subscribe(x => deviceProvider.Device = new RaspberryPi(lowLevelApi, x.Disk));
        }

        public IEnumerable<DiskViewModel> Disks => disks.Value;

        public CommandWrapper<Unit, ICollection<Disk>> RefreshDisksCommandWrapper { get; set; }

        public DiskViewModel SelectedDisk
        {
            get => selectedDisk;
            set => this.RaiseAndSetIfChanged(ref selectedDisk, value);
        }

        public bool IsBusy => isBusy.Value;

        private async Task Deploy()
        {
            if (await uiServices.DialogService.ShowConfirmation(this, Resources.DeploymentConfirmationTitle, string.Format(Resources.DeploymentConfirmationMessage, SelectedDisk)) == DialogResult.No)
            {
                return;               
            }

            var windowsDeploymentOptions = new WindowsDeploymentOptions
            {
                ImagePath = wimPickViewModel.WimMetadata.Path,
                ImageIndex = wimPickViewModel.WimMetadata.SelectedDiskImage.Index,
                SizeReservedForWindows = advancedViewModel.SizeReservedForWindows,
                UseCompact = advancedViewModel.UseCompactDeployment,
            };

            optionsProvider.Options = windowsDeploymentOptions;

            await deployer.Deploy();

            var messageViewModel = new MessageViewModel(Resources.WindowsDeployedSuccessfullyTitle, Resources.WindowsDeployedSuccessfully);
            uiServices.ViewService.Show("MarkdownViewer", messageViewModel);
        }

        public CommandWrapper<Unit, Unit> FullInstallWrapper { get; set; }
        public IObservable<bool> IsBusyObservable { get; }
    }    
}
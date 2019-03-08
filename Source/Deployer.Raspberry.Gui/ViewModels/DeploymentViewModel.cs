using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Deployer.Gui;
using Deployer.Gui.Services;
using Deployer.Gui.ViewModels;
using ReactiveUI;
using Serilog;

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
                ReactiveCommand.CreateFromTask(Deploy, isSelectedWim), uiServices.Dialog);
            IsBusyObservable = FullInstallWrapper.Command.IsExecuting;
            isBusy = IsBusyObservable.ToProperty(this, model => model.IsBusy);

            RefreshDisksCommandWrapper = new CommandWrapper<Unit, ICollection<Disk>>(this,
                ReactiveCommand.CreateFromTask(lowLevelApi.GetDisks), uiServices.Dialog);
            disks = RefreshDisksCommandWrapper.Command
                .Select(x => x.Select(disk => new DiskViewModel(disk)))
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
            if (await uiServices.Dialog.ShowConfirmation(this, Resources.DeploymentConfirmationTitle, string.Format(Resources.DeploymentConfirmationMessage, SelectedDisk)) == DialogResult.No)
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

            Log.Information("Deployment successful");


            await uiServices.Dialog.PickOptions(Resources.WindowsDeployedSuccessfully, new List<Option>()
            {
                new Option("Close")
            });
        }

        public CommandWrapper<Unit, Unit> FullInstallWrapper { get; set; }
        public IObservable<bool> IsBusyObservable { get; }
    }    
}
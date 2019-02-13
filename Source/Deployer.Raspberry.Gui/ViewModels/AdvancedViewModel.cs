using System;
using System.Reactive;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Gui.Common;
using Deployer.Gui.Core;
using ReactiveUI;

namespace Deployer.Raspberry.Gui.ViewModels
{
    public class AdvancedViewModel : ReactiveObject, IBusy
    {
        private readonly UIServices uiServices;
        private readonly ISettingsService settingsService;
        private readonly IWoaDeployer autoDeployer;
        public CommandWrapper<Unit, Unit> InstallGpuWrapper { get; set; }

        private readonly ObservableAsPropertyHelper<ByteSize> sizeReservedForWindows;

        public AdvancedViewModel(UIServices uiServices, ISettingsService settingsService, IWoaDeployer autoDeployer)
        {
            this.uiServices = uiServices;
            this.settingsService = settingsService;
            this.autoDeployer = autoDeployer;
     

            sizeReservedForWindows =
                this.WhenAnyValue(x => x.GbsReservedForWindows, ByteSize.FromGigaBytes)
                    .ToProperty(this, x => x.SizeReservedForWindows);

            IsBusyObservable = InstallGpuWrapper.Command.IsExecuting;
        }

        public ByteSize SizeReservedForWindows => sizeReservedForWindows.Value;

        public double GbsReservedForWindows
        {
            get => settingsService.SizeReservedForWindows;
            set
            {
                settingsService.SizeReservedForWindows = value;
                settingsService.Save();
                this.RaisePropertyChanged(nameof(GbsReservedForWindows));
            }
        }



        public IObservable<bool> IsBusyObservable { get; }

        public bool UseCompactDeployment
        {
            get => settingsService.UseCompactDeployment;
            set
            {
                settingsService.UseCompactDeployment = value;
                settingsService.Save();
                this.RaisePropertyChanged(nameof(UseCompactDeployment));
            }
        }
    }
}
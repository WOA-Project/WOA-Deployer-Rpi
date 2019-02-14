using System;
using System.Reactive;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Gui.Common;
using ReactiveUI;

namespace Deployer.Lumia.Gui.ViewModels
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
            InstallGpuWrapper = new CommandWrapper<Unit, Unit>(this,
                ReactiveCommand.CreateFromTask(InstallGpu), uiServices.DialogService);

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

        private async Task InstallGpu()
        {
            try
            {
                await autoDeployer.InstallGpu();
                var messageViewModel =
                    new MessageViewModel(Resources.ManualStepsTitle, Resources.InstallGpuManualSteps);

                uiServices.ViewService.Show("MarkdownViewer", messageViewModel);
            }
            catch (InvalidOperationException)
            {
                throw new ApplicationException(Resources.PhoneIsNotLumia950XL);
            }
            catch (Exception)
            {
                throw new ApplicationException(Resources.CannotInstallGpu);
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
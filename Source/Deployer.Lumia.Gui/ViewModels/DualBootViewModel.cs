using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Deployer.Gui.Core;
using ReactiveUI;

namespace Deployer.Lumia.Gui.ViewModels
{
    public class DualBootViewModel : ReactiveObject, IBusy
    {
        private readonly IPhone phone;
        private bool isCapable;
        private bool isEnabled;
        private bool isUpdated;

        public DualBootViewModel(IPhone phone, IDialogService dialogService)
        {
            this.phone = phone;
            var isChangingDualBoot = new Subject<bool>();

            UpdateStatusWrapper =
                new CommandWrapper<Unit, DualBootStatus>(this, ReactiveCommand.CreateFromTask(GetStatus, isChangingDualBoot),
                    dialogService);

            UpdateStatusWrapper.Command.Subscribe(x =>
            {
                IsCapable = x.CanDualBoot;
                IsEnabled = x.IsEnabled;
                IsUpdated = true;
            });

            var canChangeDualBoot = UpdateStatusWrapper.Command.IsExecuting.Select(isExecuting => !isExecuting);

            EnableDualBootWrapper = new CommandWrapper<Unit, Unit>(this,
                ReactiveCommand.CreateFromTask(EnableDualBoot,
                    this.WhenAnyValue(x => x.IsCapable, x => x.IsEnabled,
                            (isCapable, isEnabled) => isCapable && !isEnabled)
                        .Merge(canChangeDualBoot)), dialogService);
            EnableDualBootWrapper.Command.Subscribe(async _ =>
            {
                await dialogService.ShowAlert(this, Resources.Done, Resources.DualBootEnabled);
                IsEnabled = !IsEnabled;
            });

            DisableDualBootWrapper = new CommandWrapper<Unit, Unit>(this,
                ReactiveCommand.CreateFromTask(DisableDualBoot,
                    this.WhenAnyValue(x => x.IsCapable, x => x.IsEnabled,
                            (isCapable, isEnabled) => isCapable && isEnabled)
                        .Merge(canChangeDualBoot)), dialogService);

            DisableDualBootWrapper.Command.Subscribe(async _ =>
            {
                await dialogService.ShowAlert(this, Resources.Done, Resources.DualBootDisabled);
                IsEnabled = !IsEnabled;
            });

            
            DisableDualBootWrapper.Command.IsExecuting.Select(x => !x).Subscribe(isChangingDualBoot);
            EnableDualBootWrapper.Command.IsExecuting.Select(x => !x).Subscribe(isChangingDualBoot);

            IsBusyObservable = Observable.Merge(DisableDualBootWrapper.Command.IsExecuting,
                EnableDualBootWrapper.Command.IsExecuting, UpdateStatusWrapper.Command.IsExecuting);
        }

        public CommandWrapper<Unit, Unit> DisableDualBootWrapper { get; set; }

        public CommandWrapper<Unit, Unit> EnableDualBootWrapper { get; set; }

        public CommandWrapper<Unit, DualBootStatus> UpdateStatusWrapper { get; }

        public bool IsCapable
        {
            get => isCapable;
            set => this.RaiseAndSetIfChanged(ref isCapable, value);
        }

        public bool IsEnabled
        {
            get => isEnabled;
            set => this.RaiseAndSetIfChanged(ref isEnabled, value);
        }

        public bool IsUpdated
        {
            get => isUpdated;
            set => this.RaiseAndSetIfChanged(ref isUpdated, value);
        }

        private async Task EnableDualBoot()
        {
            await phone.EnableDualBoot(true);
        }

        private async Task DisableDualBoot()
        {
            await phone.EnableDualBoot(false);
        }

        private async Task<DualBootStatus> GetStatus()
        {
            var status = await phone.GetDualBootStatus();
         
            return status;
        }

        public IObservable<bool> IsBusyObservable { get; }
    }
}
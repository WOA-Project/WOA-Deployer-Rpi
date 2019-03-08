using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Deployer.Gui;
using ReactiveUI;

namespace Deployer.Raspberry.Gui.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly ObservableAsPropertyHelper<bool> isBusyHelper;
        private const string DonationLink = "https://github.com/WOA-Project/WOA-Deployer-Rpi/blob/master/Docs/Donations.md";
        private const string HelpLink = "https://github.com/WOA-Project/WOA-Deployer-Rpi#need-help";

        public MainViewModel(IFileSystemOperations fileSystemOperations, IEnumerable<IBusy> busies)
        {
            this.fileSystemOperations = fileSystemOperations;
            var isBusyObs = busies.Select(x => x.IsBusyObservable).Merge();

            DonateCommand = ReactiveCommand.Create(() => { Process.Start(DonationLink); });
            HelpCommand = ReactiveCommand.Create(() => { Process.Start(HelpLink); });
            OpenLogFolder = ReactiveCommand.Create(OpenLogs);
            isBusyHelper = isBusyObs.ToProperty(this, model => model.IsBusy);
        }

        private void OpenLogs()
        {
            fileSystemOperations.EnsureDirectoryExists("Logs");
            Process.Start("Logs");
        }

        public bool IsBusy => isBusyHelper.Value;

        public ReactiveCommand<Unit, Unit> DonateCommand { get; }

        public string Title => string.Format(Resources.AppTitle, AppVersionMixin.VersionString);

        public ReactiveCommand<Unit, Unit> OpenLogFolder { get; }

        public ReactiveCommand<Unit, Unit> HelpCommand { get; set; }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Deployer.Tasks;
using Deployer.UI;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using ReactiveUI;

namespace Deployer.Raspberry.Gui.ViewModels
{
    [Metadata("Name", "Advanced")]
    [Metadata("Order", 2)]
    public class AdvancedViewModel : ReactiveObject, ISection
    {
        private const string LogsZipName = "PhoneLogs.zip";
        private readonly IDeploymentContext context;
        private readonly ILogCollector logCollector;
        private readonly IRaspberryPiSettingsService raspberryPiSettingsService;
        private readonly UIServices uiServices;

        public AdvancedViewModel(IRaspberryPiSettingsService raspberryPiSettingsService, IFileSystemOperations fileSystemOperations,
            UIServices uiServices, IDeploymentContext context, IOperationContext operationContext,
            ILogCollector logCollector)
        {
            this.raspberryPiSettingsService = raspberryPiSettingsService;
            this.uiServices = uiServices;
            this.context = context;
            this.logCollector = logCollector;

            DeleteDownloadedWrapper = new CommandWrapper<Unit, Unit>(this,
                ReactiveCommand.CreateFromTask(() => DeleteDownloaded(fileSystemOperations)), uiServices.ContextDialog, operationContext);
            
            CollectLogsCommmandWrapper = new CommandWrapper<Unit, Unit>(this, ReactiveCommand.CreateFromTask(CollectLogs), uiServices.ContextDialog, operationContext);

            IsBusyObservable = Observable.Merge(DeleteDownloadedWrapper.Command.IsExecuting,
                CollectLogsCommmandWrapper.Command.IsExecuting);
        }

        private async Task CollectLogs()
        {
            await logCollector.Collect(context.Device, LogsZipName);
            var fileInfo = new FileInfo(LogsZipName);
            ExploreFile(fileInfo.FullName);
        }

        private void ExploreFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            Process.Start("explorer.exe", $"/select,\"{filePath}\"");
        }

        public CommandWrapper<Unit, Unit> DeleteDownloadedWrapper { get; }

        public bool UseCompactDeployment
        {
            get => raspberryPiSettingsService.UseCompactDeployment;
            set
            {
                raspberryPiSettingsService.UseCompactDeployment = value;
                this.RaisePropertyChanged(nameof(UseCompactDeployment));
            }
        }

        public bool CleanDownloadedBeforeDeployment
        {
            get => raspberryPiSettingsService.CleanDownloadedBeforeDeployment;
            set
            {
                raspberryPiSettingsService.CleanDownloadedBeforeDeployment = value;
                this.RaisePropertyChanged(nameof(CleanDownloadedBeforeDeployment));
            }
        }

        public CommandWrapper<Unit, Unit> CollectLogsCommmandWrapper { get; }

        public IObservable<bool> IsBusyObservable { get; }

        private async Task DeleteDownloaded(IFileSystemOperations fileSystemOperations)
        {
            if (fileSystemOperations.DirectoryExists(AppPaths.ArtifactDownload))
            {
                await fileSystemOperations.DeleteDirectory(AppPaths.ArtifactDownload);
                await uiServices.ContextDialog.ShowAlert(this, Resources.Done, UI.Properties.Resources.DownloadedFolderDeleted);
            }
            else
            {
                await uiServices.ContextDialog.ShowAlert(this, UI.Properties.Resources.DownloadedFolderNotFoundTitle,
                    UI.Properties.Resources.DownloadedFolderNotFound);
            }
        }
    }
}
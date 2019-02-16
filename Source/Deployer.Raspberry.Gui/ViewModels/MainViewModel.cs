using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Deployer.Gui.Common;
using DynamicData;
using ReactiveUI;
using Serilog.Events;

namespace Deployer.Raspberry.Gui.ViewModels
{
    public class MainViewModel : ReactiveObject, IDisposable
    {
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly ObservableAsPropertyHelper<bool> isProgressVisibleHelper;
        private readonly ObservableAsPropertyHelper<double> progressHelper;
        private ReadOnlyObservableCollection<RenderedLogEvent> logEvents;
        private IDisposable logLoader;

        private ObservableAsPropertyHelper<RenderedLogEvent> statusHelper;
        private readonly ObservableAsPropertyHelper<bool> isBusyHelper;
        private const string DonationLink = "https://github.com/WoA-project/WOA-Deployer/blob/master/Docs/Donations.md";
        private const string HelpLink = "https://github.com/WOA-Project/WOA-Deployer-Rpi#need-help";

        public MainViewModel(IObservable<LogEvent> events, 
            IObservable<double> progressSubject, IFileSystemOperations fileSystemOperations, IEnumerable<IBusy> busies)
        {
            this.fileSystemOperations = fileSystemOperations;
            progressHelper = progressSubject
                .Where(d => !double.IsNaN(d))
                .ObserveOn(SynchronizationContext.Current)
                .ToProperty(this, model => model.Progress);

            isProgressVisibleHelper = progressSubject
                .Select(d => !double.IsNaN(d))
                .ToProperty(this, x => x.IsProgressVisible);

            SetupLogging(events);

            var isBusyObs = busies.Select(x => x.IsBusyObservable).Merge();

            DonateCommand = ReactiveCommand.Create(() => { Process.Start(DonationLink); });
            HelpCommand = ReactiveCommand.Create(() => { Process.Start(HelpLink); });
            OpenLogFolder = ReactiveCommand.Create(OpenLogs);

            isBusyHelper = isBusyObs.ToProperty(this, model => model.IsBusy);
        }

        public ReactiveCommand<Unit, Unit> HelpCommand { get; set; }

        public ReactiveCommand<Unit, Unit> DonateCommand { get; }

        public bool IsBusy => isBusyHelper.Value;

        public bool IsProgressVisible => isProgressVisibleHelper.Value;

        public ReadOnlyObservableCollection<RenderedLogEvent> Events => logEvents;

        public double Progress => progressHelper.Value;

        public RenderedLogEvent Status => statusHelper.Value;

        public void Dispose()
        {
            statusHelper?.Dispose();
            logLoader?.Dispose();
            progressHelper?.Dispose();
            isProgressVisibleHelper?.Dispose();
        }

        private void OpenLogs()
        {
            fileSystemOperations.EnsureDirectoryExists("Logs");
            Process.Start("Logs");
        }

        public string Title => string.Format(Resources.AppTitle, AppVersionMixin.VersionString);

        public ReactiveCommand<Unit, Unit> OpenLogFolder { get; }

        private void SetupLogging(IObservable<LogEvent> events)
        {
            var conn = events
                .ObserveOn(SynchronizationContext.Current)
                .Where(x => x.Level == LogEventLevel.Information)
                .Publish();

            statusHelper = conn
                .Select(RenderedLogEvent)
                .ToProperty(this, x => x.Status);

            logLoader = conn
                .ToObservableChangeSet()
                .Transform(RenderedLogEvent)
                .Bind(out logEvents)
                .DisposeMany()
                .Subscribe();

            conn.Connect();
        }

        private static RenderedLogEvent RenderedLogEvent(LogEvent x)
        {
            return new RenderedLogEvent
            {
                Message = x.RenderMessage(),
                Level = x.Level
            };
        }        
    }
}
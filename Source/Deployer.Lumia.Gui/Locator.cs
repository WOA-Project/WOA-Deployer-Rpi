using System;
using System.Reactive.Subjects;
using Deployer.Gui.Common;
using Deployer.Gui.Common.Services;
using Deployer.Gui.Core;
using Deployer.Lumia.Gui.Specifics;
using Deployer.Lumia.Gui.ViewModels;
using Deployer.Lumia.Gui.Views;
using Deployer.Lumia.NetFx;
using Deployer.Tasks;
using Grace.DependencyInjection;
using MahApps.Metro.Controls.Dialogs;
using Serilog;
using Serilog.Events;

namespace Deployer.Lumia.Gui
{
    public class Locator
    {
        private readonly DependencyInjectionContainer container;
        
        public Locator()
        {
            container = new DependencyInjectionContainer();

            IObservable<LogEvent> logEvents = null;

            IViewService viewService = new ViewService();
            viewService.Register("MarkdownViewer", typeof(MarkdownViewerWindow));

            Log.Logger = new LoggerConfiguration()
                .WriteTo.RollingFile(@"Logs\Log-{Date}.txt")
                .WriteTo.Observers(x => logEvents = x)
                .MinimumLevel.Verbose()
                .CreateLogger();

            var optionsProvider = new WindowsDeploymentOptionsProvider();

            container.Configure(x =>
            {
                x.ConfigureForTesting(optionsProvider);
                x.Export<WpfMarkdownDisplayer>().As<IMarkdownDisplayer>();
                x.ExportFactory(() => new BehaviorSubject<double>(double.NaN))
                    .As<IObserver<double>>()
                    .As<IObservable<double>>()
                    .Lifestyle.Singleton();
                x.ExportFactory(() => logEvents).As<IObservable<LogEvent>>();
                x.Export<WimPickViewModel>().ByInterfaces().As<WimPickViewModel>().Lifestyle.Singleton();
                x.Export<DualBootViewModel>().ByInterfaces().As<DualBootViewModel>().Lifestyle.Singleton();
                x.Export<AdvancedViewModel>().ByInterfaces().As<AdvancedViewModel>().Lifestyle.Singleton();
                x.Export<DeploymentViewModel>().ByInterfaces().As<DeploymentViewModel>().Lifestyle.Singleton();
                x.Export<UIServices>();
                x.ExportFactory(() => viewService).As<IViewService>();
                x.Export<DialogService>().As<IDialogService>();
                x.Export<FilePicker>().As<IFilePicker>();
                x.Export<SettingsService>().As<ISettingsService>();
                x.ExportFactory(() => DialogCoordinator.Instance).As<IDialogCoordinator>();
            });
        }

        public MainViewModel MainViewModel => container.Locate<MainViewModel>();

        public WimPickViewModel WimPickViewModel => container.Locate<WimPickViewModel>();

        public DeploymentViewModel DeploymentViewModel => container.Locate<DeploymentViewModel>();

        public AdvancedViewModel AdvancedViewModel => container.Locate<AdvancedViewModel>();

        public DualBootViewModel DualBootViewModel => container.Locate<DualBootViewModel>();
    }
}
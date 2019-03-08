using System;
using System.Reactive.Subjects;
using Deployer.FileSystem;
using Deployer.Lumia.NetFx;
using Deployer.Tasks;
using Grace.DependencyInjection;

namespace Deployer.Raspberry.Console
{
    public static class CompositionRoot
    {
        public static DependencyInjectionContainer CreateContainer(WindowsDeploymentOptionsProvider op, int diskNumber,
            Subject<double> progress)
        {
            var container = new DependencyInjectionContainer();

            container.Configure(x =>
            {
                x.Configure(op);
                x.Export<ConsoleMarkdownDialog>().As<IMarkdownDialog>();
                x.ExportFactory((ILowLevelApi lla) => new DeviceProvider() {Device = new RaspberryPi(lla, diskNumber)})
                    .As<IDeviceProvider>().Lifestyle.Singleton();
                x.Export<ConsoleMarkdownDisplayer>().As<IMarkdownDisplayer>();
                x.ExportInstance(progress).As<IObserver<double>>();
            });
            return container;
        }
    }
}
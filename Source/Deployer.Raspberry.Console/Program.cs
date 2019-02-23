using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CommandLine;
using Deployer.FileSystem;
using Deployer.Lumia.NetFx;
using Deployer.Raspberry.Console.Options;
using Deployer.Tasks;
using Grace.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Deployer.Raspberry.Console
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            ConfigureLogger();

            var progress = new Subject<double>();
            using (new ConsoleDisplayUpdater(progress))
            {
                try
                {
                    await Execute(args, progress);
                
                    Log.Information("Execution finished");

                }
                catch (Exception e)
                {
                    Log.Fatal(e, "Operation failed");
                    throw;
                }              
            }
        }
        
        private static async Task Execute(IEnumerable<string> args, Subject<double> subject)
        {
            var optionsProvider = new WindowsDeploymentOptionsProvider();
            
            var parserResult = Parser.Default
                .ParseArguments<WindowsDeploymentCmdOptions,
                        NonWindowsDeploymentCmdOptions>(args);

            await parserResult
                .MapResult(
                    (WindowsDeploymentCmdOptions opts) =>
                    {
                        var deployer = GetDeployer(optionsProvider, opts.DiskNumber, subject);
                        optionsProvider.Options = new WindowsDeploymentOptions
                        {
                            ImageIndex = opts.Index,
                            ImagePath = opts.WimImage,
                            UseCompact = opts.UseCompact,
                        };
                        return deployer.Deploy();
                    },
                    (NonWindowsDeploymentCmdOptions opts) =>
                    {
                        var deployer = GetDeployer(optionsProvider, opts.DiskNumber, subject);
                        return deployer.Deploy();
                    },
                    HandleErrors);
        }

        private static IWoaDeployer GetDeployer(WindowsDeploymentOptionsProvider op, int diskNumber, Subject<double> progress)
        {
            var container = new DependencyInjectionContainer();

            container.Configure(x =>
            {
                x.Configure(op);
                x.Export<ConsoleMarkdownDialog>().As<IMarkdownDialog>();
                x.ExportFactory((ILowLevelApi lla) => new DeviceProvider() { Device = new RaspberryPi(lla, diskNumber)}).As<IDeviceProvider>().Lifestyle.Singleton();
                x.Export<ConsoleMarkdownDisplayer>().As<IMarkdownDisplayer>();
                x.ExportInstance(progress).As<IObserver<double>>();
            });

            var deployer = container.Locate<IWoaDeployer>();
            return deployer;
        }

        private static Task HandleErrors(IEnumerable<Error> errs)
        {
            var errors = string.Join("\n", errs.Select(x => x.Tag));

            System.Console.WriteLine($@"Invalid command line: {errors}");
            return Task.CompletedTask;
        }

        private static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(LogEventLevel.Information)
                .WriteTo.RollingFile(@"Logs\Log-{Date}.txt")
                .MinimumLevel.Verbose()
                .CreateLogger();
        }
    }
}
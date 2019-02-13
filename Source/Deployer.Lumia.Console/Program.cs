using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using ByteSizeLib;
using CommandLine;
using Deployer;
using Deployer.Lumia;
using Deployer.Lumia.NetFx;
using Deployer.Tasks;
using Deployment.Console.Options;
using Grace.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Deployment.Console
{
    internal static class Program
    {
        private static async Task Main(string[] args)
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
            
            var deployer = GetDeployer(optionsProvider, subject);

            var parserResult = Parser.Default
                .ParseArguments<WindowsDeploymentCmdOptions,
                        EnableDualBootCmdOptions,
                        DisableDualBootCmdOptions,
                        InstallGpuCmdOptions,
                        NonWindowsDeploymentCmdOptions>(args);

            await parserResult
                .MapResult(
                    (WindowsDeploymentCmdOptions opts) =>
                    {
                        optionsProvider.Options = new WindowsDeploymentOptions()
                        {
                            ImageIndex = opts.Index,
                            ImagePath = opts.WimImage,
                            SizeReservedForWindows = ByteSize.FromGigaBytes(opts.ReservedSizeForWindowsInGb),
                            UseCompact = opts.UseCompact,
                        };
                        return deployer.Deploy();
                    },
                    (EnableDualBootCmdOptions opts) => deployer.ToogleDualBoot(true),
                    (DisableDualBootCmdOptions opts) => deployer.ToogleDualBoot(false),
                    (InstallGpuCmdOptions opts) => deployer.InstallGpu(),
                    (NonWindowsDeploymentCmdOptions opts) => deployer.Deploy(),
                    HandleErrors);
        }

        private static IWoaDeployer GetDeployer(WindowsDeploymentOptionsProvider op, Subject<double> progress)
        {
            var container = new DependencyInjectionContainer();

            container.Configure(x =>
            {
                x.Configure(op);
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
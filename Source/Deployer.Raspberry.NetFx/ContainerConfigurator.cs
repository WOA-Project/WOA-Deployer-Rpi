using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Deployer.DevOpsBuildClient;
using Deployer.Execution;
using Deployer.FileSystem;
using Deployer.NetFx;
using Deployer.Raspberry;
using Deployer.Services;
using Deployer.Tasks;
using Grace.DependencyInjection;
using Octokit;
using Superpower;

namespace Deployer.Lumia.NetFx
{
    public static class ContainerConfigurator
    {
        public static IExportRegistrationBlock Configure(this IExportRegistrationBlock block)
        {
            var taskTypes = from a in Assemblies.AppDomainAssemblies
                            from type in a.ExportedTypes
                            where type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IDeploymentTask))
                            select type;
            block.Export<ZipExtractor>().As<IZipExtractor>();
            block.ExportFactory(Tokenizer.Create).As<Tokenizer<LangToken>>().Lifestyle.Singleton();
            block.Export<ScriptParser>().As<IScriptParser>().Lifestyle.Singleton();
            block.Export<WoaDeployer>().As<IWoaDeployer>().Lifestyle.Singleton();
            block.Export<BootCreator>().As<IBootCreator>().Lifestyle.Singleton();
            block.Export<RaspberryPathBuilder>().As<IPathBuilder>().Lifestyle.Singleton();
            block.ExportInstance(taskTypes).As<IEnumerable<Type>>();
            block.Export<ScriptRunner>().As<IScriptRunner>().Lifestyle.Singleton();
            block.Export<InstanceBuilder>().As<IInstanceBuilder>().Lifestyle.Singleton();
            block.Export<FileSystemOperations>().As<IFileSystemOperations>().Lifestyle.Singleton();
            block.Export<BcdInvokerFactory>().As<IBcdInvokerFactory>().Lifestyle.Singleton();
            block.Export<WindowsDeployer>().As<IWindowsDeployer>().Lifestyle.Singleton();
            block.ExportFactory(() => new HttpClient { Timeout = TimeSpan.FromMinutes(30) }).Lifestyle.Singleton();
            block.ExportFactory(() => new GitHubClient(new ProductHeaderValue("WOADeployer"))).As<IGitHubClient>().Lifestyle.Singleton();
            block.Export<Downloader>().As<IDownloader>().Lifestyle.Singleton();
            block.Export<OperationContext>().As<IOperationContext>().Lifestyle.Singleton();
            block.Export<ImageFlasher>().As<IImageFlasher>().Lifestyle.Singleton();
            block.ExportFactory(() => AzureDevOpsBuildClient.Create(new Uri("https://dev.azure.com"))).As<IAzureDevOpsBuildClient>().Lifestyle.Singleton();
            block.Export<DismImageService>().As<IWindowsImageService>().Lifestyle.Singleton();
            block.Export<LogCollector>().As<ILogCollector>().Lifestyle.Singleton();
            block.Export<DiskRoot>().As<IDiskRoot>().Lifestyle.Singleton();
            block.ExportFactory(() => new DeploymentContext()
            {
                DiskLayoutPreparer = new RaspberryDisklayoutPreparer()
            }).As<IDeploymentContext>().Lifestyle.Singleton();
            
            return block;
        }
    }
}
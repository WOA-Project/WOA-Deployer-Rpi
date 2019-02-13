using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Deployer.DevOpsBuildClient;
using Deployer.Execution;
using Deployer.Filesystem.FullFx;
using Deployer.FileSystem;
using Deployer.Raspberry;
using Deployer.Services;
using Deployer.Tasks;
using Grace.DependencyInjection;
using Superpower;

namespace Deployer.Lumia.NetFx
{
    public static class ContainerConfigurator
    {
        public static IExportRegistrationBlock Configure(this IExportRegistrationBlock block,
            WindowsDeploymentOptionsProvider installOptionsProvider)
        {
            return WithCommon(block, installOptionsProvider);
        }

        public static IExportRegistrationBlock WithCommon(this IExportRegistrationBlock block,
            WindowsDeploymentOptionsProvider installOptionsProvider)
        {
            var taskTypes = from a in Assemblies.AppDomainAssemblies
                            from type in a.ExportedTypes
                            where type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IDeploymentTask))
                            select type;
            block.ExportAssemblies(Assemblies.AppDomainAssemblies).ByInterface<ISpaceAllocator<IDevice>>();
            block.Export<ZipExtractor>().As<IZipExtractor>();
            block.ExportFactory(Tokenizer.Create).As<Tokenizer<LangToken>>();
            block.Export<ScriptParser>().As<IScriptParser>();
            block.ExportFactory(() => installOptionsProvider).As<IWindowsOptionsProvider>();
            block.Export<WoaDeployer>().As<IWoaDeployer>();
            block.Export<BootCreator>().As<IBootCreator>();
            block.Export<LowLevelApi>().As<ILowLevelApi>();
            block.ExportInstance(taskTypes).As<IEnumerable<Type>>();
            block.Export<ScriptRunner>().As<IScriptRunner>();
            block.Export<InstanceBuilder>().As<IInstanceBuilder>();
            block.Export<RaspberryPathBuilder>().As<IPathBuilder>();
            block.Export<FileSystemOperations>().As<IFileSystemOperations>();
            block.Export<BcdInvokerFactory>().As<IBcdInvokerFactory>();
            block.Export<WindowsDeployer>().As<IWindowsDeployer>();
            block.Export<GitHubDownloader>().As<IGitHubDownloader>();
            block.Export<DeviceProvider>().As<IDeviceProvider>().Lifestyle.Singleton();
            block.Export<RaspberryDisklayoutPreparer>().As<IDisklayoutPreparer>();
            block.Export<ImageFlasher>().As<IImageFlasher>();
            block.Export<DismImageService>().As<IWindowsImageService>();

            block.ExportFactory(() => AzureDevOpsClient.Create(new Uri("https://dev.azure.com"))).As<IAzureDevOpsBuildClient>();         

            return block;
        }
    }
}
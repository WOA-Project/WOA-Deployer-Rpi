using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Deployer.DevOpsBuildClient;
using Deployer.Execution;
using Deployer.Filesystem.FullFx;
using Deployer.FileSystem;
using Deployer.Lumia.NetFx.PhoneInfo;
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
            return WithCommon(block, installOptionsProvider).WithRealPhone();
        }

        public static IExportRegistrationBlock ConfigureForTesting(this IExportRegistrationBlock block,
            WindowsDeploymentOptionsProvider installOptionsProvider)
        {
            return WithCommon(block, installOptionsProvider).WithTestingPhone();
        }

        public static IExportRegistrationBlock WithCommon(this IExportRegistrationBlock block,
            WindowsDeploymentOptionsProvider installOptionsProvider)
        {
            var taskTypes = from a in Assemblies.AppDomainAssemblies
                            from type in a.ExportedTypes
                            where type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IDeploymentTask))
                            select type;
            block.ExportAssemblies(Assemblies.AppDomainAssemblies).ByInterface<ISpaceAllocator<IPhone>>();
            block.Export<ZipExtractor>().As<IZipExtractor>();
            block.ExportFactory(Tokenizer.Create).As<Tokenizer<LangToken>>();
            block.Export<ScriptParser>().As<IScriptParser>();
            block.ExportFactory(() => installOptionsProvider).As<IWindowsOptionsProvider>();
            block.Export<LumiaDisklayoutPreparer>().As<IDisklayoutPreparer>();
            block.Export<PhoneInfoReader>().As<IPhoneInfoReader>();
            block.Export<WoaDeployer>().As<IWoaDeployer>();
            block.Export<Tooling>().As<ITooling>();
            block.Export<BootCreator>().As<IBootCreator>();
            block.Export<LowLevelApi>().As<ILowLevelApi>();
            block.Export<PhonePathBuilder>().As<IPathBuilder>();
            block.ExportInstance(taskTypes).As<IEnumerable<Type>>();
            block.Export<ScriptRunner>().As<IScriptRunner>();
            block.Export<InstanceBuilder>().As<IInstanceBuilder>();
            block.ExportFactory((IPhone p) => new DeviceProvider { Device = p }).As<IDeviceProvider>();

            block.Export<FileSystemOperations>().As<IFileSystemOperations>();
            block.Export<BcdInvokerFactory>().As<IBcdInvokerFactory>();
            block.Export<WindowsDeployer>().As<IWindowsDeployer>();
            block.Export<GitHubDownloader>().As<IGitHubDownloader>();

            WithRealPhone(block);

            block.ExportFactory(() => AzureDevOpsClient.Create(new Uri("https://dev.azure.com"))).As<IAzureDevOpsBuildClient>();

            return block;
        }

        private static IExportRegistrationBlock WithRealPhone(this IExportRegistrationBlock block)
        {
            block.Export<PhoneModelReader>().As<IPhoneModelReader>();
            block.Export<Phone>().As<IPhone>().As<IDevice>();
            block.Export<DismImageService>().As<IWindowsImageService>();
            return block;
        }

        private static IExportRegistrationBlock WithTestingPhone(this IExportRegistrationBlock block)
        {
            block.Export<TestPhoneModelReader>().As<IPhoneModelReader>();
            block.Export<TestPhone>().As<IPhone>().As<IDevice>();
            block.Export<TestImageService>().As<IWindowsImageService>();

            return block;
        }
    }
}
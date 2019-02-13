using System;
using System.Threading.Tasks;
using Deployer.DevOpsBuildClient;
using Deployer.Tasks;
using Xunit;

namespace Deployer.Tests.Tasks
{
    public class AzureDevOpsUnpackTests
    {
        [Fact(Skip = "Long running")]
        public async Task Test()
        {
            var zipExtractor = new ZipExtractor(new FileSystemOperations());
            var azureDevOpsClient = AzureDevOpsClient.Create(new Uri("https://dev.azure.com"));
            var task = new AzureDevOpsUnpack("LumiaWOA;Lumia950XLPkg;1;MSM8994 UEFI (Lumia 950 XL)", azureDevOpsClient, zipExtractor);
            await task.Execute();
        }
    }
}
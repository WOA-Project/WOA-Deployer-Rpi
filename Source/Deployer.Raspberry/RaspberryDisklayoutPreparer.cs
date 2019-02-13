using System;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Deployer.Services;
using Deployer.Tasks;
using Serilog;

namespace Deployer.Raspberry
{
    public class RaspberryDisklayoutPreparer : IDisklayoutPreparer
    {
        private readonly IImageFlasher imageFlasher;
        private readonly IObserver<double> progressObserver;

        public RaspberryDisklayoutPreparer(IImageFlasher imageFlasher, IObserver<double> progressObserver)
        {
            this.imageFlasher = imageFlasher;
            this.progressObserver = progressObserver;
        }

        public async Task Prepare(Disk disk)
        {
            Log.Information("Flashing GPT image...");
            await imageFlasher.Flash(disk, @"Core\gpt.zip", progressObserver);
            Log.Information("GPT image flashed");
        }
    }
}
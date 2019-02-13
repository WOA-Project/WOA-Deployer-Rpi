using System;
using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer.Services
{
    public interface IImageFlasher
    {
        Task Flash(Disk disk, string imagePath, IObserver<double> progressObserver = null);
    }
}
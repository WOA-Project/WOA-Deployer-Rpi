using System;
using System.Threading.Tasks;

namespace Deployer.Services
{
    public interface IPackageImporter
    {
        Task Extract(string packagePath, IObserver<double> progressObserver = null);
        Task<string> GetReadmeText(string fileName);
    }
}
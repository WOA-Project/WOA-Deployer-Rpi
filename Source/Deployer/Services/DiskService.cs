using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer.Services
{
    public class DiskService
    {
        private readonly ILowLevelApi api;

        public DiskService(ILowLevelApi api)
        {
            this.api = api;
        }

        public Task<ICollection<Disk>> GetDisks()
        {
            return api.GetDisks();
        }
    }
}
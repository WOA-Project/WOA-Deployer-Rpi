using System.Collections.Generic;

namespace Deployer.Services.Wim
{
    public class XmlWindowsImageMetadata
    {
        public IList<DiskImageMetadata> Images { get; set; }
    }
}
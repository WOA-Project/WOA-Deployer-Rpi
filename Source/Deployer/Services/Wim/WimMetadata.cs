using System.Collections.Generic;
using System.Xml.Serialization;

namespace Deployer.Services.Wim
{
    [XmlRoot(ElementName = "WIM")]
    public class WimMetadata
    {
        [XmlElement(ElementName = "TOTALBYTES")]
        public string TotalBytes { get; set; }

        [XmlElement(ElementName = "IMAGE")] public List<ImageMetadata> Images { get; set; }
    }
}
using System.Xml.Serialization;

namespace Deployer.Services.Wim
{
    [XmlRoot(ElementName = "SERVICINGDATA")]
    public class ServicingData
    {
        [XmlElement(ElementName = "GDRDUREVISION")]
        public string GdrDuRevision { get; set; }

        [XmlElement(ElementName = "PKEYCONFIGVERSION")]
        public string PKeyConfigVersion { get; set; }
    }
}
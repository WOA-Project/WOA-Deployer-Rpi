using System.Xml.Serialization;

namespace Deployer.Services.Wim
{
    [XmlRoot(ElementName = "VERSION")]
    public class Version
    {
        [XmlElement(ElementName = "MAJOR")] public string Major { get; set; }

        [XmlElement(ElementName = "MINOR")] public string Minor { get; set; }

        [XmlElement(ElementName = "BUILD")] public string Build { get; set; }

        [XmlElement(ElementName = "SPBUILD")] public string SpBuild { get; set; }

        [XmlElement(ElementName = "SPLEVEL")] public string SpLevel { get; set; }
    }
}
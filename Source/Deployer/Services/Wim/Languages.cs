using System.Xml.Serialization;

namespace Deployer.Services.Wim
{
    [XmlRoot(ElementName = "LANGUAGES")]
    public class Languages
    {
        [XmlElement(ElementName = "LANGUAGE")] public string Language { get; set; }

        [XmlElement(ElementName = "DEFAULT")] public string Default { get; set; }
    }
}
using System.Xml.Serialization;

namespace Deployer.Services.Wim
{
    [XmlRoot(ElementName = "WINDOWS")]
    public class Windows
    {
        [XmlElement(ElementName = "ARCH")] public string Arch { get; set; }

        [XmlElement(ElementName = "PRODUCTNAME")]
        public string ProductName { get; set; }

        [XmlElement(ElementName = "EDITIONID")]
        public string EditionId { get; set; }

        [XmlElement(ElementName = "INSTALLATIONTYPE")]
        public string InstallationType { get; set; }

        [XmlElement(ElementName = "SERVICINGDATA")]
        public ServicingData ServicingData { get; set; }

        [XmlElement(ElementName = "PRODUCTTYPE")]
        public string ProductType { get; set; }

        [XmlElement(ElementName = "PRODUCTSUITE")]
        public string ProductSuite { get; set; }

        [XmlElement(ElementName = "LANGUAGES")]
        public Languages Languages { get; set; }

        [XmlElement(ElementName = "VERSION")] public Version Version { get; set; }

        [XmlElement(ElementName = "SYSTEMROOT")]
        public string SystemRoot { get; set; }
    }
}
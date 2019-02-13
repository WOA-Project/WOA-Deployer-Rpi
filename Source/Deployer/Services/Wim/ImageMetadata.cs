using System.Xml.Serialization;

namespace Deployer.Services.Wim
{
    [XmlRoot(ElementName = "IMAGE")]
    public class ImageMetadata
    {
        [XmlElement(ElementName = "DIRCOUNT")] public string DirectoryCount { get; set; }

        [XmlElement(ElementName = "FILECOUNT")]
        public string FileCount { get; set; }

        [XmlElement(ElementName = "TOTALBYTES")]
        public string TotalBytes { get; set; }

        [XmlElement(ElementName = "HARDLINKBYTES")]
        public string HardLinkBytes { get; set; }

        [XmlElement(ElementName = "CREATIONTIME")]
        public Time CreationTime { get; set; }

        [XmlElement(ElementName = "LASTMODIFICATIONTIME")]
        public Time LastModificationTime { get; set; }

        [XmlElement(ElementName = "WIMBOOT")] public string WimBoot { get; set; }

        [XmlElement(ElementName = "WINDOWS")] public Windows Windows { get; set; }

        [XmlElement(ElementName = "NAME")] public string Name { get; set; }

        [XmlElement(ElementName = "DESCRIPTION")]
        public string Description { get; set; }

        [XmlElement(ElementName = "FLAGS")] public string Flags { get; set; }

        [XmlElement(ElementName = "DISPLAYNAME")]
        public string DiplayName { get; set; }

        [XmlElement(ElementName = "DISPLAYDESCRIPTION")]
        public string DisplayDescription { get; set; }

        [XmlAttribute(AttributeName = "INDEX")]
        public string Index { get; set; }
    }
}
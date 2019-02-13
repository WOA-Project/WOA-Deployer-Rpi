using System.IO;

namespace Deployer.Services.Wim
{
    public interface IWindowsImageMetadataReader
    {
        XmlWindowsImageMetadata Load(Stream stream);
    }
}
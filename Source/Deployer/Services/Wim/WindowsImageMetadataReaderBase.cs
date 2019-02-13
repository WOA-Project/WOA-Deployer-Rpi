using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using Deployer.Exceptions;
using Serilog;

namespace Deployer.Services.Wim
{
    public abstract class WindowsImageMetadataReaderBase : IWindowsImageMetadataReader
    {
        private static XmlSerializer Serializer { get; } = new XmlSerializer(typeof(WimMetadata));

        public XmlWindowsImageMetadata Load(Stream stream)
        {
            Log.Verbose("Getting WIM stream");

            WimMetadata metadata;
            try
            {
                metadata = (WimMetadata)Serializer.Deserialize(GetXmlMetadataStream(stream));
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidWimFileException("Could not read the metadata from the WIM " +
                    "file. Please, check it's a valid .WIM file", e);
            }

            Log.Verbose("Wim metadata deserialized correctly {@Metadata}", metadata);

            return new XmlWindowsImageMetadata
            {
                Images = metadata.Images
                    .Where(x => x.Windows != null)
                    .Select(x => new DiskImageMetadata
                {
                    Architecture = GetArchitecture(x.Windows.Arch),
                    Build = x.Windows.Version.Build,
                    DisplayName = x.Name,
                    Index = int.Parse(x.Index)
                }).ToList()
            };
        }

        private static Architecture GetArchitecture(string str)
        {
            switch (str)
            {
                case "0":
                    return Architecture.X86;
                case "9":
                    return Architecture.X64;
                case "12":
                    return Architecture.Arm64;
            }

            throw new IndexOutOfRangeException($"The architecture '{str}' is unknown");
        }

        protected abstract Stream GetXmlMetadataStream(Stream wim);
    }
}
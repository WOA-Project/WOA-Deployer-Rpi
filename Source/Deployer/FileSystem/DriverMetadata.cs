using System;

namespace Deployer.FileSystem
{
    public class DriverMetadata
    {
        public string Driver { get; set; }
        public string OriginalFileName { get; set; }
        public bool Inbox { get; set; }
        public bool BootCritical { get; set; }
        public string ProviderName { get; set; }
        public DateTime Date { get; set; }
        public string Version { get; set; }
    }
}
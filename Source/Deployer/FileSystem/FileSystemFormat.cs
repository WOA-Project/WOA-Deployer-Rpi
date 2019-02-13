namespace Deployer.FileSystem
{
    public class FileSystemFormat
    {
        public string Moniker { get; }

        public static FileSystemFormat  Ntfs = new FileSystemFormat("NTFS");
        public static FileSystemFormat  Fat16 = new FileSystemFormat("FAT");
        public static FileSystemFormat  Fat32 = new FileSystemFormat("FAT32");

        private FileSystemFormat(string moniker)
        {
            Moniker = moniker;
        }
    }
}
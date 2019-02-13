 namespace Deployer.Gui.Core
{
    public class FileTypeFilter
    {
        public string Description { get; }
        public string[] Extensions { get; }

        public FileTypeFilter(string description, params string[] extensions)
        {
            Description = description;
            Extensions = extensions;
        }
    }
}
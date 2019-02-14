using System.Collections.Generic;

namespace Deployer.Gui.Common
{
    public interface IFilePicker
    {
        string InitialDirectory { get; set; }
        List<FileTypeFilter> FileTypeFilter { get; }
        string PickFile();
    }
}
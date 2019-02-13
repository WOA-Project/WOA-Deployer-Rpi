using System.Collections.Generic;

namespace Deployer.Gui.Core
{
    public interface IFilePicker
    {
        string InitialDirectory { get; set; }
        List<FileTypeFilter> FileTypeFilter { get; }
        string PickFile();
    }
}
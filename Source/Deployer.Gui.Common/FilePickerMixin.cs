using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;

namespace Deployer.Gui.Common
{
    public static class FilePickerMixin
    {
        public static string Pick(this IFilePicker openFileService, IEnumerable<(string, IEnumerable<string>)> extensions, Func<string> getCurrentFolder, Action<string> setCurrentFolder)
        {
            var fileTypeFilters = extensions.Select(tuple => new FileTypeFilter(tuple.Item1, tuple.Item2.ToArray()));

            openFileService.FileTypeFilter.Clear();
            openFileService.FileTypeFilter.AddRange(fileTypeFilters);

            openFileService.InitialDirectory = getCurrentFolder();

            var selected = openFileService.PickFile();

            if (selected != null)
            {
                var directoryName = Path.GetDirectoryName(selected);
                setCurrentFolder(directoryName);
                Log.Verbose("Default directory for WimFolder has been set to {Folder}", directoryName);
            }

            return selected;
        }
    }
}
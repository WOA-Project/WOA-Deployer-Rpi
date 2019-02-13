using System;
using System.Collections.Generic;
using System.Linq;
using Deployer.Services.Wim;
using ReactiveUI;

namespace Deployer.Gui.Common
{
    public class WimMetadataViewModel : ReactiveObject
    {
        private DiskImageMetadata selectedDiskImage;

        public WimMetadataViewModel(XmlWindowsImageMetadata windowsImageMetadata, string path)
        {
            WindowsImageMetadata = windowsImageMetadata;
            Path = path;
            SelectedImageObs = this.WhenAnyValue(x => x.SelectedDiskImage);
            SelectedDiskImage = Images.First();
        }

        private XmlWindowsImageMetadata WindowsImageMetadata { get; }
        public string Path { get; }

        public IObservable<DiskImageMetadata> SelectedImageObs { get; }

        public ICollection<DiskImageMetadata> Images => WindowsImageMetadata.Images;

        public DiskImageMetadata SelectedDiskImage
        {
            get => selectedDiskImage;
            set => this.RaiseAndSetIfChanged(ref selectedDiskImage, value);
        }
     
    }
}
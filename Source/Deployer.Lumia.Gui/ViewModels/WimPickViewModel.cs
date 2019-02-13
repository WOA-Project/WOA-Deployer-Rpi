using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Deployer.Exceptions;
using Deployer.Gui.Common;
using Deployer.Gui.Core;
using Deployer.Services.Wim;
using ReactiveUI;
using Serilog;

namespace Deployer.Lumia.Gui.ViewModels
{
    public class WimPickViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<bool> hasWimHelper;

        private readonly ObservableAsPropertyHelper<WimMetadataViewModel> pickWimFileObs;
        private readonly ISettingsService settingsService;
        private readonly UIServices uiServices;

        public WimPickViewModel(UIServices uiServices, ISettingsService settingsService)
        {
            this.uiServices = uiServices;
            this.settingsService = settingsService;

            PickWimFileCommand = ReactiveCommand.CreateFromObservable(() => PickWimFileObs);
            pickWimFileObs = PickWimFileCommand.ToProperty(this, x => x.WimMetadata);
            PickWimFileCommand.ThrownExceptions.Subscribe(e =>
            {
                Log.Error(e, "WIM file error");
                this.uiServices.DialogService.ShowAlert(this, Resources.InvalidWimFile, e.Message);
            });

            hasWimHelper = this.WhenAnyValue(model => model.WimMetadata, (WimMetadataViewModel x) => x != null)
                .ToProperty(this, x => x.HasWim);
        }

        public ReactiveCommand<Unit, WimMetadataViewModel> PickWimFileCommand { get; set; }

        public WimMetadataViewModel WimMetadata => pickWimFileObs.Value;

        private IObservable<WimMetadataViewModel> PickWimFileObs
        {
            get
            {
                var value = uiServices.FilePicker.Pick(
                    new List<(string, IEnumerable<string>)> {(Resources.WimFilesFilter, new[] {"install.wim"})},
                    () => settingsService.WimFolder, x =>
                    {
                        settingsService.WimFolder = x;
                        settingsService.Save();
                    });

                return Observable.Return(value).Where(x => x != null)
                    .Select(LoadWimMetadata);
            }
        }

        public bool HasWim => hasWimHelper.Value;

        private static WimMetadataViewModel LoadWimMetadata(string path)
        {
            Log.Verbose("Trying to load WIM metadata file at '{ImagePath}'", path);

            using (var file = File.OpenRead(path))
            {
                var imageReader = new WindowsImageMetadataReader();
                var windowsImageInfo = imageReader.Load(file);
                if (windowsImageInfo.Images.All(x => x.Architecture != Architecture.Arm64))
                {
                    throw new InvalidWimFileException(Resources.WimFileNoValidArchitecture);
                }

                var vm = new WimMetadataViewModel(windowsImageInfo, path);

                Log.Verbose("WIM metadata file at '{ImagePath}' retrieved correctly", path);

                return vm;
            }
        }
    }
}
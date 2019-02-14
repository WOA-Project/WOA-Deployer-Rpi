namespace Deployer.Gui.Common
{
    public class UIServices
    {
        public UIServices(IFilePicker filePicker, IViewService viewService, IDialogService dialogService)
        {
            FilePicker = filePicker;
            ViewService = viewService;
            DialogService = dialogService;
        }

        public IFilePicker FilePicker { get; }
        public IViewService ViewService { get; }
        public IDialogService DialogService { get; }
    }
}
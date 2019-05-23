using Deployer.Raspberry.Gui.Properties;

namespace Deployer.Raspberry.Gui.Specifics
{
    public class RaspberryPiSettingsService : IRaspberryPiSettingsService
    {
        private readonly Settings settings = Settings.Default;

        public string WimFolder
        {
            get => Settings.Default.WimFolder;
            set => Settings.Default.WimFolder = value;
        }

        public bool UseCompactDeployment
        {
            get => Settings.Default.UseCompactDeployment;
            set => Settings.Default.UseCompactDeployment = value;
        }

        public bool CleanDownloadedBeforeDeployment
        {
            get => settings.CleanDownloadedBeforeDeployment;
            set
            {
                settings.CleanDownloadedBeforeDeployment = value;
                settings.Save();
            }
        }

        public void Save()
        {
            Settings.Default.Save();
        }
    }
}
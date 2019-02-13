using Deployer.Gui.Common;
using Deployer.Raspberry.Gui.Properties;

namespace Deployer.Raspberry.Gui.Specifics
{
    public class SettingsService : ISettingsService
    {
        public string WimFolder
        {
            get => Settings.Default.WimFolder;
            set => Settings.Default.WimFolder = value;
        }

        public double SizeReservedForWindows
        {
            get => Settings.Default.SizeReservedForWindows;
            set => Settings.Default.SizeReservedForWindows = value;
        }

        public bool UseCompactDeployment
        {
            get => Settings.Default.UseCompactDeployment;
            set => Settings.Default.UseCompactDeployment = value;
        }

        public void Save()
        {
            Settings.Default.Save();
        }
    }
}
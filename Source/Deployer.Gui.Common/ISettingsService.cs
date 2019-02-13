namespace Deployer.Gui.Common
{
    public interface ISettingsService
    {
        string WimFolder { get; set; }
        double SizeReservedForWindows { get; set; }
        bool UseCompactDeployment { get; set; }
        void Save();
    }
}
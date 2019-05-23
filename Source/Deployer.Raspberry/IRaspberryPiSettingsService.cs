namespace Deployer.Raspberry
{
    public interface IRaspberryPiSettingsService : ISettingsService
    {
        bool UseCompactDeployment { get; set; }
        bool CleanDownloadedBeforeDeployment { get; set; }
    }
}
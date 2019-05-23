namespace Deployer.Raspberry.Gui
{
    public class AppProperties
    {
        public const string GitHubBaseUrl = "https://github.com/WOA-Project/WOA-Deployer-Rpi";
        public static string AppTitle => string.Format(Resources.AppTitle, AppVersionMixin.VersionString);
    }
}
using System.Reflection;

namespace Deployer
{
    public class AppVersionMixin
    {
        public static string VersionString
        {
            get
            {
                var version = Assembly.GetEntryAssembly().GetName().Version;
                return $"{version.Major}.{version.Minor}.{version.Revision}";
            }
        }
    }
}
using System.Windows;
using Serilog;

namespace Deployer.Lumia.Gui
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);  
            
            MahApps.Metro.ThemeManager.IsAutomaticWindowsAppModeSettingSyncEnabled = true;
            MahApps.Metro.ThemeManager.SyncThemeWithWindowsAppModeSetting();
        }
    }
}

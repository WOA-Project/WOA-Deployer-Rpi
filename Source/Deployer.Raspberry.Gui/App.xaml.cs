using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Deployer.Gui.Common;
using Deployer.Raspberry.Console;
using Deployer.Raspberry.Gui.Views;

namespace Deployer.Raspberry.Gui
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MahApps.Metro.ThemeManager.IsAutomaticWindowsAppModeSettingSyncEnabled = true;
            MahApps.Metro.ThemeManager.SyncThemeWithWindowsAppModeSetting();

            if (e.Args.Any())
            {
                LaunchConsole(e.Args);
            }
            else
            {
                LaunchGui();
            }
        }

        private void LaunchGui()
        {
            var window = new MainWindow();
            MainWindow = window;
            window.Show();            
        }

        private void LaunchConsole(string[] args)
        {
            UpdateChecker.CheckForUpdates(AppProperties.GitHubBaseUrl);
            
            ConsoleEmbedder.ExecuteInsideConsole(() => Task.Run(() => Program.Main(args)).Wait());
            Shutdown();
        }
    }
}

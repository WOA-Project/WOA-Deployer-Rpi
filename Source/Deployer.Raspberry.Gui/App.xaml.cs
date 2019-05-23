using System.Linq;
using System.Windows;
using Deployer.Raspberry.Gui.Views;
using Deployer.UI;

namespace Deployer.Raspberry.Gui
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MahApps.Metro.ThemeManager.IsAutomaticWindowsAppModeSettingSyncEnabled = true;
            MahApps.Metro.ThemeManager.SyncThemeWithWindowsAppModeSetting();
            UpdateChecker.CheckForUpdates(AppProperties.GitHubBaseUrl);

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
            //ConsoleEmbedder.ExecuteInsideConsole(() => Task.Run(() => Program.Main(args)).Wait());
            //Shutdown();
        }
    }
}

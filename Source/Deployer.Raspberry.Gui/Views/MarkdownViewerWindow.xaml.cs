using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Deployer.Raspberry.Gui.Views
{
    
    public partial class MarkdownViewerWindow
    {
        public MarkdownViewerWindow()
        {
            InitializeComponent();
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start((string) e.Parameter);
        }
    }
}

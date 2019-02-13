using System.Windows;

namespace Deployer.Lumia.Gui.Views
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
    }
}

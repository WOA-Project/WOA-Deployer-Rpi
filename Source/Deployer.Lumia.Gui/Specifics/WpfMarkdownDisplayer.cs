using System.Threading.Tasks;
using Deployer.Gui.Common;
using Deployer.Lumia.Gui.ViewModels;
using Deployer.Tasks;

namespace Deployer.Lumia.Gui.Specifics
{
    public class WpfMarkdownDisplayer : IMarkdownDisplayer
    {
        private readonly UIServices services;

        public WpfMarkdownDisplayer(UIServices services)
        {
            this.services = services;
        }

        public Task Display(string title, string message)
        {
            services.ViewService.Show("MarkdownViewer", new MessageViewModel(title, message));
            return Task.CompletedTask;
        }
    }
}
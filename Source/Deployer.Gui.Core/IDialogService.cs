using System.Threading.Tasks;

namespace Deployer.Gui.Core
{
    public interface IDialogService
    {
        Task ShowAlert(object owner, string title, string text);
    }
}
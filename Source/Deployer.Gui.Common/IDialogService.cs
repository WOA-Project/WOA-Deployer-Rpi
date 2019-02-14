using System.Threading.Tasks;

namespace Deployer.Gui.Common
{
    public interface IDialogService
    {
        Task ShowAlert(object owner, string title, string text);
    }
}
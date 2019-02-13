using System.Threading.Tasks;

namespace Deployer.Tasks
{
    public interface IMarkdownDisplayer
    {
        Task Display(string title, string message);
    }
}
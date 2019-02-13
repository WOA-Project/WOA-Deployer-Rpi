using System.Threading.Tasks;

namespace Deployer
{
    public interface IInstanceBuilderProvider
    {
        Task<IInstanceBuilder> Create();
    }
}
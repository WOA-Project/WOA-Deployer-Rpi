using Deployer.Services;

namespace Deployer
{
    public interface IBcdInvokerFactory
    {
        IBcdInvoker Create(string path);
    }    
}
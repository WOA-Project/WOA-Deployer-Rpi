using Deployer.Services;

namespace Deployer
{
    public class BcdInvokerFactory : IBcdInvokerFactory
    {
        public IBcdInvoker Create(string path)
        {
            return new BcdInvoker(path);
        }
    }
}
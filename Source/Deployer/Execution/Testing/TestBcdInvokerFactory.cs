using Deployer.Services;

namespace Deployer.Execution.Testing
{
    public class TestBcdInvokerFactory : IBcdInvokerFactory
    {
        public IBcdInvoker Create(string path)
        {
            return new TestBcdInvoker();
        }
    }
}
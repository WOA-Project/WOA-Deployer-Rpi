namespace Deployer.Services
{
    public interface IBcdInvoker
    {
        string Invoke(string command = "");
    }
}
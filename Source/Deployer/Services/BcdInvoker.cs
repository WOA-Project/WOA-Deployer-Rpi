using Deployer.Utils;

namespace Deployer.Services
{
    public class BcdInvoker : IBcdInvoker
    {
        private readonly string commonArgs;
        private readonly string bcdEdit;

        public BcdInvoker(string store)
        {
            bcdEdit = WindowsCommandLineUtils.BcdEdit;
            commonArgs = $@"/STORE ""{store}""";
        }

        public string Invoke(string command)
        {
            return ProcessUtils.Run(bcdEdit, $@"{commonArgs} {command}");
        }
    }
}
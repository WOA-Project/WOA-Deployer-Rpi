using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer.Raspberry.Console
{
    internal class ConsoleMarkdownDialog : IDialog
    {
        public Task<Option> PickOptions(string markdown, IEnumerable<Option> options, string assetBasePath = "")
        {
            System.Console.WriteLine(
                @"By continuing you are accepting the following license below.
If you decline it, press Control+C anytime during the deployment process.
" + markdown);
            return Task.FromResult(new Option("Accept", OptionValue.OK));
        }

        public Task<DialogResult> Show(string key, object context)
        {
            throw new System.NotImplementedException();
        }
    }
}
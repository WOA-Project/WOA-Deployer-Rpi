using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Lumia
{
    public class WoaDeployer : IWoaDeployer
    {
        private readonly ITooling tooling;
        private readonly IPhone phone;
        private readonly IScriptRunner scriptRunner;
        private readonly IScriptParser parser;

        public WoaDeployer(IScriptRunner scriptRunner, IScriptParser parser, ITooling tooling, IPhone phone)
        {
            this.scriptRunner = scriptRunner;
            this.parser = parser;
            this.tooling = tooling;
            this.phone = phone;
        }

        public async Task Deploy()
        {
            var dict = new Dictionary<PhoneModel, string>
            {
                {PhoneModel.Talkman, Path.Combine("Scripts", "950.txt")},
                {PhoneModel.Cityman, Path.Combine("Scripts", "950xl.txt")},
            };

            var phoneModel = await phone.GetModel();
            var path = dict[phoneModel];

            await scriptRunner.Run(parser.Parse(File.ReadAllText(path)));
        }

        public async Task InstallGpu()
        {
            await tooling.InstallGpu();
        }

        public Task ToogleDualBoot(bool isEnabled)
        {
            return tooling.ToogleDualBoot(isEnabled);
        }
    }
}
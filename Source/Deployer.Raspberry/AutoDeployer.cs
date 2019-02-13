using System.IO;
using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Raspberry
{
    public class WoaDeployer : IWoaDeployer
    {
        private readonly ITooling tooling;
        private readonly IScriptRunner scriptRunner;
        private readonly IScriptParser parser;

        public WoaDeployer(IScriptRunner scriptRunner, IScriptParser parser, ITooling tooling)
        {
            this.scriptRunner = scriptRunner;
            this.parser = parser;
            this.tooling = tooling;
        }

        public async Task Deploy()
        {
            var scriptPath = Path.Combine("Scripts", "Deployment.txt");
            
            await scriptRunner.Run(parser.Parse(File.ReadAllText(scriptPath)));
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
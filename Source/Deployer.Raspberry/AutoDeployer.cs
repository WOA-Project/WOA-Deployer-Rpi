using System.IO;
using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Raspberry
{
    public class WoaDeployer : IWoaDeployer
    {
        private readonly IScriptRunner scriptRunner;
        private readonly IScriptParser parser;

        public WoaDeployer(IScriptRunner scriptRunner, IScriptParser parser)
        {
            this.scriptRunner = scriptRunner;
            this.parser = parser;
        }

        public async Task Deploy()
        {
            var scriptPath = Path.Combine("Scripts", "Deployment.txt");
            
            await scriptRunner.Run(parser.Parse(File.ReadAllText(scriptPath)));
        }        
    }
}
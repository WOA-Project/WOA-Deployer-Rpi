using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer.Execution;
using Deployer.FileSystem;
using Deployer.Tasks;
using Serilog;

namespace Deployer.Lumia.NetFx
{
    // ReSharper disable once ClassNeverInstantiated.Local
    public class Tooling : ITooling
    {
        private readonly IPhone phone;
        private readonly IScriptRunner scriptRunner;

        public Tooling(IPhone phone, IScriptRunner scriptRunner)
        {
            this.phone = phone;
            this.scriptRunner = scriptRunner;
        }

        public async Task ToogleDualBoot(bool isEnabled)
        {
            var enabledStr = isEnabled ? "Enabling" : "Disabling";
            Log.Information($"{enabledStr} Dual Boot");
            await phone.EnableDualBoot(isEnabled);

            Log.Information("Done");
        }

        public async Task InstallGpu()
        {
            if (await phone.GetModel() != PhoneModel.Cityman)
            {
                var ex = new InvalidOperationException("This phone is not a Lumia 950 XL");
                Log.Error(ex, "Phone isn't a Lumia 950 XL");
                
                throw ex;
            }

            Log.Information("Installing GPU");
            await phone.EnsureBootPartitionIs(PartitionType.Basic);

            IList<Sentence> sentences = new List<Sentence>()
            {
                new Sentence(new Command(nameof(GitHubUnpack), new[] {new Argument("https://github.com/gus33000/MSM8994-8992-NT-ARM64-Drivers"),})),
                new Sentence(new Command(nameof(CopyDirectory), new[]
                {
                    new Argument(@"Downloaded\MSM8994-8992-NT-ARM64-Drivers-master\Supplemental\GPU\Cityman"),
                    new Argument(@"WindowsARM\Users\Public\OEMPanel"),
                })),
            };

            await scriptRunner.Run(new Script(sentences));                
        }        
    }
}
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Deployer.Tasks;

namespace Deployer.Raspberry
{
    public class RaspberryPathBuilder : IPathBuilder
    {
        private readonly IDeploymentContext context;

        public RaspberryPathBuilder(IDeploymentContext context)
        {
            this.context = context;
        }

        public async Task<string> Replace(string str)
        {
            IDictionary<string, Func<Task<string>>> mappings = new Dictionary<string, Func<Task<string>>>()
            {
                { @"\[Windows\]", async () => (await context.Device.GetWindowsPartition()).Root },
                { @"\[System\]", async () => (await context.Device.GetSystemPartition()).Root },
            };

            foreach (var mapping in mappings)
            {
                if (Regex.IsMatch(str, mapping.Key))
                {
                    var mappingValue = await mapping.Value();
                    str = Regex.Replace(str, $"^{mapping.Key}", mappingValue, RegexOptions.IgnoreCase);
                    str = Regex.Replace(str, $@"\\+", @"\", RegexOptions.IgnoreCase);
                }
            }

            return str;
        }
    }
}
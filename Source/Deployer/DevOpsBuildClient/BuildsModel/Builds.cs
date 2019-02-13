using System.Collections.Generic;
using Newtonsoft.Json;

namespace Deployer.DevOpsBuildClient.BuildsModel
{
    public class Builds
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public IList<Build> Value { get; set; }
    }
}
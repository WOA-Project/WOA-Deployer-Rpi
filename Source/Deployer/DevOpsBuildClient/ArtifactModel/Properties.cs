using Newtonsoft.Json;

namespace Deployer.DevOpsBuildClient.ArtifactModel
{
    public class Properties
    {

        [JsonProperty("localpath")]
        public string Localpath { get; set; }
    }
}

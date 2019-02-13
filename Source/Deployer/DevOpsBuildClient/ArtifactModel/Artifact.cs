using Newtonsoft.Json;

namespace Deployer.DevOpsBuildClient.ArtifactModel
{
    public class Artifact
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("resource")]
        public Resource Resource { get; set; }
    }
}
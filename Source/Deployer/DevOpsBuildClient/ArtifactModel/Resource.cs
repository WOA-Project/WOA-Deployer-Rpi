using Newtonsoft.Json;

namespace Deployer.DevOpsBuildClient.ArtifactModel
{
    public class Resource
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }
    }
}
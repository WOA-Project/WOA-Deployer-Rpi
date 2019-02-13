using Newtonsoft.Json;

namespace Deployer.DevOpsBuildClient.BuildsModel
{
    public class Logs
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
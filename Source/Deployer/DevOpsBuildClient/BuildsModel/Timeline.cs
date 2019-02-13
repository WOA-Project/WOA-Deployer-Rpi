using Newtonsoft.Json;

namespace Deployer.DevOpsBuildClient.BuildsModel
{
    public class Timeline
    {

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}
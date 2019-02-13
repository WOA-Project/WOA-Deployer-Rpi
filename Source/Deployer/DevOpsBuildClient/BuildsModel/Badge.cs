using Newtonsoft.Json;

namespace Deployer.DevOpsBuildClient.BuildsModel
{
    public class Badge
    {

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}
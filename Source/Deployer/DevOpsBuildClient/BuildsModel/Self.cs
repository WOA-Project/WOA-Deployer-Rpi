using Newtonsoft.Json;

namespace Deployer.DevOpsBuildClient.BuildsModel
{
    public class Self
    {

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}

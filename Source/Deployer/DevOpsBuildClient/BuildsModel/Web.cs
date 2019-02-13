using Newtonsoft.Json;

namespace Deployer.DevOpsBuildClient.BuildsModel
{
    public class Web
    {

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}
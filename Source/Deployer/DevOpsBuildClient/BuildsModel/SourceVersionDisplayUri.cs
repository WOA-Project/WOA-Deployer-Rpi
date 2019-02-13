using Newtonsoft.Json;

namespace Deployer.DevOpsBuildClient.BuildsModel
{
    public class SourceVersionDisplayUri
    {

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}
using Newtonsoft.Json;

namespace Deployer.DevOpsBuildClient.BuildsModel
{
    public class Plan
    {

        [JsonProperty("planId")]
        public string PlanId { get; set; }
    }
}
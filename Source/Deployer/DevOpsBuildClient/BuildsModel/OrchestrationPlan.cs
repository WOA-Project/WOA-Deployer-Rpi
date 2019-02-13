using Newtonsoft.Json;

namespace Deployer.DevOpsBuildClient.BuildsModel
{
    public class OrchestrationPlan
    {

        [JsonProperty("planId")]
        public string PlanId { get; set; }
    }
}
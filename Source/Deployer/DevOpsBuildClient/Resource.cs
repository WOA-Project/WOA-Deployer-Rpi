namespace Deployer.DevOpsBuildClient
{
    public class Resource
    {
        public string type { get; set; }
        public string data { get; set; }
        public Properties properties { get; set; }
        public string url { get; set; }
        public string downloadUrl { get; set; }
    }
}
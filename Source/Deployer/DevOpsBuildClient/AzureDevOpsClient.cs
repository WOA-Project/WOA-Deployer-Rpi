using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Deployer.DevOpsBuildClient.ArtifactModel;
using Deployer.DevOpsBuildClient.BuildsModel;
using Refit;

namespace Deployer.DevOpsBuildClient
{
    public class AzureDevOpsClient : IAzureDevOpsBuildClient
    {
        private readonly IBuildApiClient inner;

        public AzureDevOpsClient(IBuildApiClient inner)
        {
            this.inner = inner;
        }

        public static AzureDevOpsClient Create(Uri baseAddress)
        {
            var httpClient = new HttpClient(new CustomHttpClientHandler()
            {
                
            })
            {
                BaseAddress = baseAddress,                
            };


            return new AzureDevOpsClient(RestService.For<IBuildApiClient>(httpClient));
        }

        public Task<Artifact> GetArtifact(string org, string project, int buildId, string artifactsName)
        {
            return inner.GetArtifact(org, project, buildId, artifactsName);
        }

        public async Task<Build> GetLatestBuild(string org, string project, int definition)
        {
            var builds = await inner.GetLatestBuild(org, project, definition);
            return builds.Value.First();
        }

        public async Task<Artifact> LatestBuildArtifact(string org, string project, int definitionId, string artifactsName)
        {
            var build = await GetLatestBuild(org, project, definitionId);
            var artifact = await GetArtifact(org, project, build.Id, artifactsName);
            return artifact;
        }
    }
}
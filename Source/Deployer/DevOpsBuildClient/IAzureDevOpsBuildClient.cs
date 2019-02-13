using System.Threading.Tasks;
using Deployer.DevOpsBuildClient.ArtifactModel;
using Deployer.DevOpsBuildClient.BuildsModel;

namespace Deployer.DevOpsBuildClient
{
    public interface IAzureDevOpsBuildClient
    {
        Task<Artifact> GetArtifact(string org, string project, int buildId, string artifactsName);
        Task<Build> GetLatestBuild(string org, string project, int definition);
        Task<Artifact> LatestBuildArtifact(string org, string project, int definitionId, string artifactsName);
    }
}
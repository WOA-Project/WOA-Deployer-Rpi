using System.Threading.Tasks;
using Deployer.DevOpsBuildClient.ArtifactModel;
using Deployer.DevOpsBuildClient.BuildsModel;
using Refit;

namespace Deployer.DevOpsBuildClient
{
    public interface IBuildApiClient
    {
        [Get("/{org}/{project}/_apis/build/builds/{buildId}/artifacts?artifactName={artifactName}&api-version=5.0-preview.5")]
        Task<Artifact> GetArtifact(string org, string project, int buildId, string artifactName);

        //https://dev.azure.com//LumiaWOA/Boot%20Shim/_apis/build/builds?definitions=3&$top=1&api-version=5.0-preview.5
        [Get("/{org}/{project}/_apis/build/builds?definitions={definition}&$top=1&api-version=5.0-preview.5")]
        Task<Builds> GetLatestBuild(string org, string project, int definition);
    }
}
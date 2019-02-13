namespace Deployer.Execution
{
    public interface IScriptParser
    {
        Script Parse(string input);
    }
}
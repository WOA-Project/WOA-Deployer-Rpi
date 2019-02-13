namespace Deployer.Execution
{
    public class Sentence
    {
        public Command Command { get; }

        public Sentence(Command command)
        {
            Command = command;
        }

        public override string ToString()
        {
            return $"{Command}";
        }
    }
}
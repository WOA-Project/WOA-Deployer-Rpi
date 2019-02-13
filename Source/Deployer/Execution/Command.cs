using System.Collections.Generic;

namespace Deployer.Execution
{
    public class Command
    {
        public string Name { get; }
        public IEnumerable<Argument> Arguments { get; }

        public Command(string name, IEnumerable<Argument> arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public override string ToString()
        {
            return $"{Name}({string.Join(",", Arguments)})";
        }
    }
}
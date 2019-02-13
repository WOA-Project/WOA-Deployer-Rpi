namespace Deployer.Execution
{
    public class Argument
    {
        public Argument(object value)
        {
            Value = value;
        }

        public object Value { get; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
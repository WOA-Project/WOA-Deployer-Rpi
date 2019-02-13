using System;
using Deployer.Services;
using Serilog;

namespace Deployer.Execution.Testing
{
    public class TestBcdInvoker : IBcdInvoker
    {
        public string Invoke(string command)
        {
            Log.Verbose("Invoked BCDEdit: '{Command}'", command);
            if (command.Contains("/create"))
                return Guid.NewGuid().ToString();

            return $"Executed '{command}'";
        }
    }
}
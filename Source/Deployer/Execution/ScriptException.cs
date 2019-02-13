using System;

namespace Deployer.Execution
{
    internal class ScriptException : Exception
    {
        public ScriptException(string msg) : base(msg)
        {            
        }
    }
}
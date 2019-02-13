using System;

namespace Deployer.Exceptions
{
    public class DeploymentException : Exception
    {
        public DeploymentException(string msg) : base(msg)
        {            
        }
    }
}
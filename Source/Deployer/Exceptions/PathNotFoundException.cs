using System;

namespace Deployer.Exceptions
{
    internal class PathNotFoundException : Exception
    {
        public PathNotFoundException(string msg) : base(msg)
        {
        }
    }
}
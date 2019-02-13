using System;

namespace Deployer.Exceptions
{
    public class NotEnoughSpaceException : Exception
    {
        public NotEnoughSpaceException(string msg) : base(msg)
        {
        }
    }
}
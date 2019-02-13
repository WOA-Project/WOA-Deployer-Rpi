using System;

namespace Deployer.Exceptions
{
    public class InvalidWimFileException : Exception
    {
        public InvalidWimFileException(string msg) : base(msg)
        {            
        }

        public InvalidWimFileException(string msg, Exception innerException) : base(msg, innerException)
        {            
        }
    }
}
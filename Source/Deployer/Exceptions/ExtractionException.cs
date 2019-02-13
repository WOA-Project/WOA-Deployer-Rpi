using System;

namespace Deployer.Exceptions
{
    internal class ExtractionException : Exception
    {
        public ExtractionException(string msg) : base(msg)
        {            
        }
    }
}
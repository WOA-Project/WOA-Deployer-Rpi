using System;

namespace Deployer.Filesystem.FullFx
{
    internal class InvalidImageException : Exception
    {
        public InvalidImageException(string msg) : base(msg)
        {            
        }
    }
}
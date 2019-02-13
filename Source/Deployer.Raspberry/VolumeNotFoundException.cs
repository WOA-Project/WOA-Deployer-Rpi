using System;

namespace Deployer.Raspberry
{
    public class VolumeNotFoundException : ApplicationException
    {
        public VolumeNotFoundException(string msg) : base(msg)
        {        
        }
    }
}
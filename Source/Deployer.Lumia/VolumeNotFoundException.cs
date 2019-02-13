using System;

namespace Deployer.Lumia
{
    public class VolumeNotFoundException : ApplicationException
    {
        public VolumeNotFoundException(string msg) : base(msg)
        {        
        }
    }
}
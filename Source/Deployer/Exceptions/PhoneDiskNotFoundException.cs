using System;

namespace Deployer.Exceptions
{
    public class PhoneDiskNotFoundException : Exception
    {
        public PhoneDiskNotFoundException(string message)  : base(message)
        {
            
        }
    }
}
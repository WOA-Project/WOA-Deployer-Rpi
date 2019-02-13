using System;

namespace Deployer
{
    public interface IInstanceBuilder
    {
        object Create(Type type, params object[] arguments);
    }
}
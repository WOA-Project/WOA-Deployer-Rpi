using System.Collections.Generic;
using Grace.DependencyInjection;

namespace Deployer.Tests
{
    internal class TestInstanceBuilder : InstanceBuilder
    {
        private readonly List<object> createdInstances = new List<object>();

        public TestInstanceBuilder(ILocatorService container) : base(container)
        {
        }

        public IReadOnlyCollection<object> CreatedInstances => createdInstances.AsReadOnly();

        protected override void OnInstanceCreated(object instance)
        {
            createdInstances.Add(instance);
        }
    }
}
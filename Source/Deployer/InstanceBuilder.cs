using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.DependencyInjection;

namespace Deployer
{
    public class InstanceBuilder : IInstanceBuilder
    {
        private readonly ILocatorService container;

        public InstanceBuilder(ILocatorService container)
        {
            this.container = container;
        }

        public object Create(Type type, params object[] arguments)
        {
            var finalArguments = GetArguments(type, arguments);

            var instance = Activator.CreateInstance(type, finalArguments.ToArray());
            OnInstanceCreated(instance);
            return instance;
        }

        private IEnumerable<object> GetArguments(Type type, IEnumerable<object> arguments)
        {
            var ctor = type.GetTypeInfo().DeclaredConstructors.First();

            var zipped = ctor.GetParameters().Select(x => x.ParameterType).ZipLongest(arguments.Take(ctor.GetParameters().Length), (pInfo, val) => (info: pInfo, o: val));

            return from tuple in zipped
                let final = ConvertParam(tuple.o, tuple.info)
                select final;
        }

        private object ConvertParam(object value, Type paramType)
        {
            if (paramType == typeof(string))
            {
                if (value == null)
                {
                    throw new InvalidOperationException("Invalid arguments provided");
                }

                return value;
            }

            return container.Locate(paramType);
        }

        protected virtual void OnInstanceCreated(object instance)
        {            
        }
    }
}
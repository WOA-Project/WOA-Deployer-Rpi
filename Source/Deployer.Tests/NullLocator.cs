using System;
using System.Collections.Generic;
using Grace.DependencyInjection;

namespace Deployer.Tests
{
    public class NullLocator : ILocatorService
    {
        public bool CanLocate(Type type, ActivationStrategyFilter consider = null, object key = null)
        {
            throw new NotImplementedException();
        }

        public object Locate(Type type)
        {
            throw new NotImplementedException();
        }

        public object LocateOrDefault(Type type, object defaultValue)
        {
            throw new NotImplementedException();
        }

        public T Locate<T>()
        {
            throw new NotImplementedException();
        }

        public T LocateOrDefault<T>(T defaultValue = default(T))
        {
            throw new NotImplementedException();
        }

        public object Locate(Type type, object extraData = null, ActivationStrategyFilter consider = null, object withKey = null,
            bool isDynamic = false)
        {
            throw new NotImplementedException();
        }

        public T Locate<T>(object extraData = null, ActivationStrategyFilter consider = null, object withKey = null,
            bool isDynamic = false)
        {
            throw new NotImplementedException();
        }

        public List<object> LocateAll(Type type, object extraData = null, ActivationStrategyFilter consider = null, IComparer<object> comparer = null)
        {
            throw new NotImplementedException();
        }

        public List<T> LocateAll<T>(Type type = null, object extraData = null, ActivationStrategyFilter consider = null,
            IComparer<T> comparer = null)
        {
            throw new NotImplementedException();
        }

        public bool TryLocate<T>(out T value, object extraData = null, ActivationStrategyFilter consider = null, object withKey = null,
            bool isDynamic = false)
        {
            throw new NotImplementedException();
        }

        public bool TryLocate(Type type, out object value, object extraData = null, ActivationStrategyFilter consider = null,
            object withKey = null, bool isDynamic = false)
        {
            throw new NotImplementedException();
        }

        public object LocateByName(string name, object extraData = null, ActivationStrategyFilter consider = null)
        {
            throw new NotImplementedException();
        }

        public List<object> LocateAllByName(string name, object extraData = null, ActivationStrategyFilter consider = null)
        {
            throw new NotImplementedException();
        }

        public bool TryLocateByName(string name, out object value, object extraData = null, ActivationStrategyFilter consider = null)
        {
            throw new NotImplementedException();
        }
    }
}
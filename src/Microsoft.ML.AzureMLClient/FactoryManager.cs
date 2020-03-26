// <copyright file="FactoryManager.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Concurrent;

namespace Azure.MachineLearning.Services
{
    public class FactoryManager<T>
    {
        private ConcurrentDictionary<string, T> factories
            = new ConcurrentDictionary<string, T>();

        public FactoryManager()
        {
        }

        public T DefaultFactory { get; set; }

        public void RegisterFactory(string factoryType, T factory)
        {
            {
                this.factories[factoryType] = factory;
            }
        }

        public T GetFactory(string factoryType)
        {
            // No check on targetType, since we have a DefaultFactory
            T result = this.DefaultFactory;

            if (factoryType != null)
            {
                if (this.factories.ContainsKey(factoryType))
                {
                    result = this.factories[factoryType];
                }
            }

            return result;
        }
    }
}

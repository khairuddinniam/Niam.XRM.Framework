using System;
using System.Collections.Generic;
using Niam.XRM.Framework.Interfaces.Plugin.ServiceProviders;

namespace Niam.XRM.Framework.Plugin.ServiceProviders
{
    internal class Container : IServiceProvider, IContainer, IDisposable
    {
        private readonly IDictionary<Type, Func<IContainer, object>> _registeredObjects = new Dictionary<Type, Func<IContainer, object>>(); 
        private readonly IList<IDisposable> _disposables = new List<IDisposable>();
        private readonly IServiceProvider _serviceProvider;

        public Container(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            if (typeof (IServiceProvider) == serviceType) return this;

            return _registeredObjects.TryGetValue(serviceType, out var serviceFactory)
                ? serviceFactory(this)
                : _serviceProvider.GetService(serviceType);
        }

        public void Register<TRegisterType>(TRegisterType instance)
            where TRegisterType : class =>
            Register(typeof (TRegisterType), instance);

        public void Register(Type registerType, object instance)
        {
            Register(registerType, c => instance);

            if (instance is IDisposable disposeable)
                _disposables.Add(disposeable);
        }

        public void Register<TRegisterType>(Func<IContainer, TRegisterType> instanceFactory)
            where TRegisterType : class =>
            Register(typeof (TRegisterType), instanceFactory);

        public void Register(Type registerType, Func<IContainer, object> instanceFactory) =>
            _registeredObjects[registerType] = instanceFactory;

        public T Resolve<T>() => (T) Resolve(typeof (T));

        public object Resolve(Type type) => GetService(type);

        public void Dispose()
        {
            _registeredObjects.Clear();

            foreach (var disposable in _disposables)
                disposable.Dispose();
        }
    }
}

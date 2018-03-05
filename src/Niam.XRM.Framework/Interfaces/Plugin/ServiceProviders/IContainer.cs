using System;

namespace Niam.XRM.Framework.Interfaces.Plugin.ServiceProviders
{
    public interface IContainer
    {
        void Register<TRegisterType>(TRegisterType instance) where TRegisterType : class;
        void Register(Type registerType, object instance);
        void Register<TRegisterType>(Func<IContainer, TRegisterType> instanceFactory) where TRegisterType : class;
        void Register(Type registerType, Func<IContainer, object> instanceFactory);

        object Resolve(Type type);
        T Resolve<T>();
    }
}

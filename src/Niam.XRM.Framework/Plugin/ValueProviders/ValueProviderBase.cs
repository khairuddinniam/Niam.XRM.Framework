using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework.Plugin.ValueProviders
{
    public abstract class ValueProviderBase<T> : IValueProvider<T>
    {
        object IValueProvider.GetValue() => GetValue();

        T IValueProvider<T>.GetValue() => GetValue();

        public abstract T GetValue();
    }
}

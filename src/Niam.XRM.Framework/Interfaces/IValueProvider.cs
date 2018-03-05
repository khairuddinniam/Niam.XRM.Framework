namespace Niam.XRM.Framework.Interfaces
{
    public interface IValueProvider
    {
        object GetValue();
    }

    public interface IValueProvider<out TV> : IValueProvider
    {
        new TV GetValue();
    }
}

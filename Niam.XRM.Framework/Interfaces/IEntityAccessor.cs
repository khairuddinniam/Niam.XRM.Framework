using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Interfaces
{
    public interface IEntityAccessor<out T> : IEntityGetter<T>, IEntitySetter<T>
        where T : Entity
    {
        T Entity { get; }
    }
}

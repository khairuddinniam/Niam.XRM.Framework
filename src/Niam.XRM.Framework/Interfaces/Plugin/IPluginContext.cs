using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Data;

namespace Niam.XRM.Framework.Interfaces.Plugin
{
    public interface IPluginContext<TE> : ITransactionContext<TE>, IEntityWrapperBase<TE> , IEntityGetter<TE>, IEntitySetter<TE>
        where TE : Entity
    {
    }
}
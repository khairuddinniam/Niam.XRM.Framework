using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;

namespace Niam.XRM.Framework.Plugin.Strategy
{
    internal abstract class InternalEntityGetterBase
    {
        public abstract Entity Get<T>(ITransactionContext<T> context,
            ITransactionContextConfiguration<T> config) 
            where T : Entity;
    }
}

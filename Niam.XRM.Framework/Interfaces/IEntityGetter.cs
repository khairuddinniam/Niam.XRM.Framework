using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Interfaces
{
    public interface IEntityGetter<out T>
        where T : Entity
    {
        TV Get<TV>(string attributeName);
        string GetFormattedValue(string attributeName);
    }
}
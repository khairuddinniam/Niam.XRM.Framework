using System;
using Microsoft.Xrm.Sdk;
using AttributeCollection = Microsoft.Xrm.Sdk.AttributeCollection;

namespace Niam.XRM.Framework.Interfaces.Plugin
{
    public interface ITransactionContextEntity<out T> : IEntityAccessor<T>
        where T : Entity
    {
        event AttributeChangingEventHandler AttributeChanging;
        event AttributeChangedEventHandler AttributeChanged;
        
        Guid Id { get; set; }
        string LogicalName { get; set; }
        AttributeCollection Attributes { get; set; }
        object this[string attributeName] { get; set; }
    }
}

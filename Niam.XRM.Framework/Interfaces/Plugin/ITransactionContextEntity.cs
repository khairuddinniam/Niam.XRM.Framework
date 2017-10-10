using System;
using System.ComponentModel;
using Microsoft.Xrm.Sdk;
using AttributeCollection = Microsoft.Xrm.Sdk.AttributeCollection;

namespace Niam.XRM.Framework.Interfaces.Plugin
{
    public interface ITransactionContextEntity<out T> : IEntityAccessor<T>
        where T : Entity
    {
        EventHandlerList EventHandlers { get; }
        event PropertyChangingEventHandler AttributeChanging;
        event PropertyChangedEventHandler AttributeChanged;
        
        Guid Id { get; set; }
        string LogicalName { get; set; }
        AttributeCollection Attributes { get; set; }
        object this[string attributeName] { get; set; }
    }
}

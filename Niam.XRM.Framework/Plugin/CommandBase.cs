using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Infrastructure;
using Niam.XRM.Framework.Interfaces;
using Niam.XRM.Framework.Interfaces.Data;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin
{
    public abstract class CommandBase<TE, TW> : IEntityWrapperRelation<TE>
        where TE : Entity
        where TW : class, IEntityWrapper<TE>
    {
        private readonly Lazy<TW> _wrapper;

        protected ITransactionContext<TE> Context { get; }
        protected IOrganizationService Service => Context.Service;
        protected IEntityGetter<TE> Initial => Context.Initial;
        
        protected virtual TW Wrapper => _wrapper.Value;
        
        private TW GetWrapper() => InstanceEntityWrapper<TE, TW>.Create(Context.Reference.Entity, Context);

        protected CommandBase(ITransactionContext<TE> context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _wrapper = new Lazy<TW>(GetWrapper);
        }

        protected bool Changed<TV>(Expression<Func<TE, TV>> attribute, TV from, TV to)
            => Changed(Helper.Name(attribute), from, to);

        protected bool Changed<TV>(string attribute, TV from, TV to)
            => Initial.Equal(attribute, from) && Equal(attribute, to);

        protected bool Changed(Expression<Func<TE, Money>> moneyAttribute, decimal? from, decimal? to)
        {
            var fromMoney = from.HasValue ? new Money(from.Value) : null;
            var toMoney = to.HasValue ? new Money(to.Value) : null;
            return Changed(moneyAttribute, fromMoney, toMoney);
        }

        protected bool Changed(Expression<Func<TE, OptionSetValue>> optionAttribute, Enum from, Enum to)
            => Changed(optionAttribute, from.ToOptionSetValue(), to.ToOptionSetValue());

        protected string GetName<TR>(Expression<Func<TE, EntityReference>> relatedAttribute)
            where TR : Entity
            => Wrapper.GetName<TR>(relatedAttribute);

        protected TV Get<TV>(Expression<Func<TE, TV>> attribute) => Wrapper.Get(attribute);

        protected TV Get<TV>(string attribute) => Wrapper.Get<TV>(attribute);

        protected decimal GetValue(Expression<Func<TE, Money>> moneyAttribute, decimal defaultValue = 0m) 
            => Wrapper.GetValue(moneyAttribute, defaultValue);

        protected TV GetValue<TV>(Expression<Func<TE, TV?>> attribute, TV defaultValue = default(TV))
            where TV : struct
            => Wrapper.GetValue(attribute, defaultValue);

        protected string GetFormattedValue(Expression<Func<TE, object>> attribute)
            => Wrapper.GetFormattedValue(attribute);

        protected string GetFormattedValue(string attributeName)
            => Wrapper.GetFormattedValue(attributeName);

        protected bool Equal<TV>(Expression<Func<TE, TV>> attribute, TV comparisonValue) 
            => Wrapper.Equal(attribute, comparisonValue);

        protected bool Equal<TV>(string attribute, TV comparisonValue)
            => Wrapper.Equal(attribute, comparisonValue);

        protected bool Equal(Expression<Func<TE, OptionSetValue>> optionAttribute, Enum option) 
            => Wrapper.Equal(optionAttribute, option);

        protected bool EqualsAny(Expression<Func<TE, OptionSetValue>> optionAttribute, Enum firstOption, params Enum[] otherOptions)
            => Wrapper.EqualsAny(optionAttribute, firstOption, otherOptions);
        
        protected IEntityWrapper<TR> GetRelated<TR>(Expression<Func<TE, EntityReference>> relatedAttribute, IColumnSet<TR> relatedColumnSet)
            where TR : Entity => Wrapper.GetRelated(relatedAttribute, relatedColumnSet);

        protected TWrapper GetRelated<TR, TWrapper>(Expression<Func<TE, EntityReference>> relatedReference, IColumnSet<TR> relatedColumnSet)
            where TR : Entity
            where TWrapper : class, IEntityWrapper<TR> 
            => Wrapper.GetRelated<TR, TWrapper>(relatedReference, relatedColumnSet);

        protected IEntityWrapper<Entity> GetRelated(string relatedAttribute, ColumnSet relatedColumnSet)
            => Wrapper.GetRelated(relatedAttribute, relatedColumnSet);

        protected IEnumerable<IEntityWrapper<TR>> GetAllRelated<TR>(Expression<Func<TR, EntityReference>> relatedToParentAttribute, IColumnSet<TR> relatedColumnSet)
            where TR : Entity => Wrapper.GetAllRelated(relatedToParentAttribute, relatedColumnSet);

        protected IEnumerable<TWrapper> GetAllRelated<TR, TWrapper>(Expression<Func<TR, EntityReference>> relatedToParentAttribute, IColumnSet<TR> relatedColumnSet)
            where TR : Entity
            where TWrapper : class, IEntityWrapper<TR> 
            => Wrapper.GetAllRelated<TR, TWrapper>(relatedToParentAttribute, relatedColumnSet);

        protected IEnumerable<IEntityWrapper<Entity>> GetAllRelated(string relatedEntityName, string relatedToParentAttribute, ColumnSet relatedColumnSet)
            => Wrapper.GetAllRelated(relatedEntityName, relatedToParentAttribute, relatedColumnSet);

        // Explicit IEntityWrapperRelation<TE> interface implementation
        IEntityWrapper<TR> IEntityWrapperRelation<TE>.GetRelated<TR>(Expression<Func<TE, EntityReference>> relatedAttribute, IColumnSet<TR> relatedColumnSet)
            => GetRelated(relatedAttribute, relatedColumnSet);
    }
}
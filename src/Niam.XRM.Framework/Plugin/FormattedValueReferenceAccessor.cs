using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin
{
    internal class FormattedValueReferenceAccessor<T> : EntityAccessor<T>
        where T : Entity
    {
        private readonly T _input;
        private readonly ITransactionContextBase _context;

        public FormattedValueReferenceAccessor(T input, T reference, ITransactionContextBase context) 
            : base(reference)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override string GetFormattedValue(string attributeName)
        { 
            var formattedValue = base.GetFormattedValue(attributeName);
            if (formattedValue != null) return formattedValue;

            var value = Get<object>(attributeName);
            if (value == null) return null;

            formattedValue = GetFormattedValue(attributeName, value);
            UpdateFormattedValue(attributeName, formattedValue);
            return formattedValue;
        }

        private string GetFormattedValue(string attributeName, object value)
        {
            switch (value)
            {
                case OptionSetValue option:
                    return GetOptionSetFormattedValue(attributeName, option);
                case EntityReference reference:
                    return GetEntityReferenceFormattedValue(reference);
                default:
                    return null;
            }
        }

        private string GetOptionSetFormattedValue(string attributeName, OptionSetValue option)
        {
            var request = new RetrieveAttributeRequest
            {
                EntityLogicalName = Entity.LogicalName,
                LogicalName = attributeName
            };

            var response = _context.Service.Execute<RetrieveAttributeResponse>(request);
            var optionSetMetadata = ((EnumAttributeMetadata) response.AttributeMetadata).OptionSet;
            var formattedValue = optionSetMetadata.Options.First(o => o.Value == option.Value)
                .Label.UserLocalizedLabel?.Label;
            return formattedValue;
        }

        private string GetEntityReferenceFormattedValue(EntityReference reference)
        {
            var primaryNameAttribute = _context.Service.GetPrimaryAttribute(reference.LogicalName);
            var columnSet = new ColumnSet(primaryNameAttribute);
            var entity = _context.Service.Retrieve(reference.LogicalName, reference.Id, columnSet);
            return entity.Get<string>(primaryNameAttribute);
        }

        private void UpdateFormattedValue(string attributeName, string formattedValue)
        {
            if (formattedValue == null) return;

            _input.FormattedValues[attributeName] = formattedValue;
            Entity.FormattedValues[attributeName] = formattedValue;
        }
    }
}

using System.ComponentModel;
using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework
{
    public class AttributeChangedEventArgs : PropertyChangedEventArgs
    {
        public string AttributeName { get; }

        public AttributeChangedEventArgs(string attributeName)
            : this(attributeName, null)
        {
        }

        public AttributeChangedEventArgs(string attributeName, string propertyName) 
            : base(propertyName)
        {
            AttributeName = attributeName;
        }
    }

    public class AttributeChangedEventArgs<T> : AttributeChangedEventArgs
        where T : Entity
    {
        private bool _havePropertyNameValue;
        private string _propertyName;

        public override string PropertyName
        {
            get
            {
                if (!_havePropertyNameValue)
                {
                    _propertyName = Helper.Info<T>().GetMemberName(AttributeName);
                    _havePropertyNameValue = true;
                }

                return _propertyName;
            }
        }

        public AttributeChangedEventArgs(string attributeName) 
            : base(attributeName)
        {
        }
    }
}
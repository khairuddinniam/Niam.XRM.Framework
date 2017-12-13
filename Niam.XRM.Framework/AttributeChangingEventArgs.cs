using System.ComponentModel;
using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework
{
    public class AttributeChangingEventArgs : PropertyChangingEventArgs
    {
        public string AttributeName { get; }

        public AttributeChangingEventArgs(string attributeName)
            : this(attributeName, null)
        {
        }

        public AttributeChangingEventArgs(string attributeName, string propertyName) 
            : base(propertyName)
        {
            AttributeName = attributeName;
        }
    }

    public class AttributeChangingEventArgs<T> : AttributeChangingEventArgs
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

        public AttributeChangingEventArgs(string attributeName) 
            : base(attributeName)
        {
        }
    }
}
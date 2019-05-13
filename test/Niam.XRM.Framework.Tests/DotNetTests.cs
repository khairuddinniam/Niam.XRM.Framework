using System;
using Xunit;

namespace Niam.XRM.Framework.Tests
{
    public class DotNetTests
    {
        private class ClassBase
        {
            public Type GetClassType()
            {
                return GetType();
            }
        }

        private class DerivedClass : ClassBase
        {
        }

        private class DerivedDerivedClass : DerivedClass
        {
        }

        [Fact]
        public void Get_type_on_base_class_return_most_derived_type()
        {
            Assert.Equal(typeof(ClassBase), new ClassBase().GetClassType());
            Assert.Equal(typeof(DerivedClass), new DerivedClass().GetClassType());
            Assert.Equal(typeof(DerivedDerivedClass), new DerivedDerivedClass().GetClassType());
        }

        private class ContructorBaseClass
        {
            public Type Type { get; }

            public ContructorBaseClass()
            {
                Type = GetType();
            }
        }

        private class ContructorDerivedClass : ContructorBaseClass
        {
        }

        private class ContructorDerivedDerivedClass : ContructorDerivedClass
        {
        }

        [Fact]
        public void Get_type_on_base_class_return_most_derived_type_constructor()
        {
            Assert.Equal(typeof(ContructorBaseClass), new ContructorBaseClass().Type);
            Assert.Equal(typeof(ContructorDerivedClass), new ContructorDerivedClass().Type);
            Assert.Equal(typeof(ContructorDerivedDerivedClass), new ContructorDerivedDerivedClass().Type);
        }
    }
}

using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Plugin;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class TransactionContextEntityExtTests
    {
        [Fact]
        public void Can_set_attribute_value()
        {
            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            var reference = new EntityReference("reference", Guid.NewGuid());

            var txEntity = new TransactionContextEntity<xts_entity>(entity);
            txEntity.Set(e => e.xts_attribute, reference);

            Assert.Equal(reference, entity["xts_attribute"]);
        }

        [Fact]
        public void Can_set_option_set_value_using_int()
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };

            var txEntity = new TransactionContextEntity<xts_entity>(entity);
            txEntity.Set(e => e.xts_optionsetvalue, 12);

            Assert.Equal(12, entity.Get(e => e.xts_optionsetvalue).Value);
        }

        private enum TestOptions
        {
            SomeValue = 47
        }

        [Fact]
        public void Can_set_option_set_value_using_enum()
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };

            var txEntity = new TransactionContextEntity<xts_entity>(entity);
            txEntity.Set(e => e.xts_optionsetvalue, TestOptions.SomeValue);

            Assert.Equal(47, entity.Get(e => e.xts_optionsetvalue).Value);
        }

        [Fact]
        public void Can_set_money_using_decimal()
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };

            var txEntity = new TransactionContextEntity<xts_entity>(entity);
            txEntity.Set(e => e.xts_money, 1234);

            Assert.Equal(1234m, entity.Get(e => e.xts_money).Value);
        }
    }
}

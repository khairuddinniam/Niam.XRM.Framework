using Microsoft.Xrm.Sdk;
using NSubstitute;
using System;
using System.Linq.Expressions;
using Niam.XRM.Framework.Interfaces;
using Xunit;

namespace Niam.XRM.Framework.Tests
{
    public class HelperEntityAccessorTests
    {
        [Fact]
        public void Can_get_attribute_empty_value_from_null_accessor()
        {
            IEntityGetter<xts_entity> accessor = null;

            Assert.Null(accessor.Get(e => e.xts_attribute));
            Assert.Null(accessor.Get(e => e.xts_withcolumnattribute));
            Assert.Null(accessor.Get(e => e.xts_money));
            Assert.Null(accessor.Get(e => e.xts_optionsetvalue));
        }
        [Fact]
        public void Can_get_attribute_value_generic()
        {
            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            var reference = new EntityReference("reference", Guid.NewGuid());
            entity["xts_attribute"] = reference;

            var accessor = new EntityAccessor<xts_entity>(entity);
            Assert.Equal(reference, accessor.Get(e => e.xts_attribute));
        }

        [Fact]
        public void Can_get_formatted_value_generic()
        {
            var entity = new xts_entity();
            entity.FormattedValues["xts_optionsetvalue"] = "formatted-value-4567";
            var accessor = new EntityAccessor<xts_entity>(entity);
            Assert.Equal("formatted-value-4567", accessor.GetFormattedValue(e => e.xts_optionsetvalue));
            Assert.Null(accessor.GetFormattedValue(e => e.xts_decimal));
        }

        [Fact]
        public void Can_get_money_value()
        {
            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            entity["xts_money"] = null;

            var accessor = new EntityAccessor<xts_entity>(entity);
            Assert.Equal(0m, accessor.GetValue(e => e.xts_money));
            Assert.Equal(10m, accessor.GetValue(e => e.xts_money, 10m));

            entity["xts_money"] = new Money(250m);
            Assert.Equal(250m, accessor.GetValue(e => e.xts_money, 250m));
        }

        [Fact]
        public void Can_get_money_value_from_null_accessor()
        {
            IEntityGetter<xts_entity> accessor = null;

            Assert.Equal(0m, accessor.GetValue(e => e.xts_money));
            Assert.Equal(10m, accessor.GetValue(e => e.xts_money, 10m));
        }

        [Fact]
        public void Can_get_value_nullable_type()
        {
            var entity = new xts_entity();
            var accessor = new EntityAccessor<xts_entity>(entity);

            Assert.Equal(0, accessor.GetValue(e => e.xts_int));
            Assert.Equal(111, accessor.GetValue(e => e.xts_int, 111));
            entity.Set(e => e.xts_int, 123);
            Assert.Equal(123, accessor.GetValue(e => e.xts_int));
            Assert.Equal(123, accessor.GetValue(e => e.xts_int, 3333));

            Assert.Equal(0m, accessor.GetValue(e => e.xts_decimal));
            Assert.Equal(1000m, accessor.GetValue(e => e.xts_decimal, 1000m));
            entity.Set(e => e.xts_decimal, 2300m);
            Assert.Equal(2300m, accessor.GetValue(e => e.xts_decimal));
            Assert.Equal(2300m, accessor.GetValue(e => e.xts_decimal, 4000m));
        }

        [Fact]
        public void Can_set_attribute_value_generic()
        {
            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            var reference = new EntityReference("reference", Guid.NewGuid());
            var accessor = new EntityAccessor<xts_entity>(entity);
            accessor.Set(e => e.xts_attribute, reference);

            Assert.Equal(reference, entity["xts_attribute"]);
        }

        [Fact]
        public void Can_set_formatted_value_generic()
        {
            var entity = new xts_entity();
            var accessor = new EntityAccessor<xts_entity>(entity);
            accessor.SetFormattedValue(e => e.xts_optionsetvalue, "formatted-value-4567");
            Assert.Equal("formatted-value-4567", entity.FormattedValues["xts_optionsetvalue"]);
        }

        [Fact]
        public void Can_set_money_from_decimal_value_provider()
        {
            var decimalValueProvider = Substitute.For<IValueProvider<decimal>>();
            decimalValueProvider.GetValue().Returns(1500m);

            var entity = new xts_entity { Id = Guid.NewGuid() };
            var accessor = new EntityAccessor<xts_entity>(entity);
            accessor.Set(e => e.xts_money, decimalValueProvider);

            Assert.Equal(1500m, entity.Get(e => e.xts_money).Value);
        }

        [Fact]
        public void Can_set_money_using_decimal()
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };
            var accessor = new EntityAccessor<xts_entity>(entity);
            accessor.Set(e => e.xts_money, 1234);
            Assert.Equal(1234m, accessor.Get(e => e.xts_money).Value);
        }

        [Fact]
        public void Can_set_nullable_value_type_through_value_provider_generic_value_type()
        {
            var intValueProvider = Substitute.For<IValueProvider<int>>();
            intValueProvider.GetValue().Returns(1234);
            var entity = new xts_entity { Id = Guid.NewGuid() };
            var accessor = new EntityAccessor<xts_entity>(entity);
            accessor.Set(e => e.xts_int, intValueProvider);

            Assert.Equal(1234, entity.Get(e => e.xts_int));

            var date = new DateTime(2017, 3, 2);
            var dateValueProvider = Substitute.For<IValueProvider<DateTime>>();
            dateValueProvider.GetValue().Returns(date);
            accessor.Set(e => e.xts_datetime, dateValueProvider);

            Assert.Equal(date, entity.Get(e => e.xts_datetime));
        }

        [Fact]
        public void Can_set_option_set_value_using_enum()
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };
            var accessor = new EntityAccessor<xts_entity>(entity);
            accessor.Set(e => e.xts_optionsetvalue, TestOptions.SomeValue);
            Assert.Equal(47, accessor.Get(e => e.xts_optionsetvalue).Value);
        }

        [Fact]
        public void Can_set_option_set_value_using_int()
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };
            var accessor = new EntityAccessor<xts_entity>(entity);
            accessor.Set(e => e.xts_optionsetvalue, 12);
            Assert.Equal(12, accessor.Get(e => e.xts_optionsetvalue).Value);
        }

        [Fact]
        public void Can_set_through_attribute_value_provider()
        {
            var valueProvider = Substitute.For<IAttributeValueProvider>();
            valueProvider.GetValueFor(Arg.Any<string>(), Arg.Any<string>()).Returns("1234");

            var entity = new Entity();
            var accessor = new EntityAccessor<Entity>(entity);
            accessor.Set("xts_attribute", valueProvider);

            Assert.Equal("1234", entity.Get<string>("xts_attribute"));
        }

        [Fact]
        public void Can_set_through_attribute_value_provider_generic()
        {
            var valueProvider = Substitute.For<IAttributeValueProvider<xts_entity, string>>();
            valueProvider.GetValueFor(Arg.Any<Expression<Func<xts_entity, string>>>()).Returns("1234");

            var entity = new xts_entity { Id = Guid.NewGuid() };
            var accessor = new EntityAccessor<xts_entity>(entity);
            accessor.Set(e => e.xts_string, valueProvider);

            Assert.Equal("1234", entity.Get(e => e.xts_string));
        }

        [Fact]
        public void Can_set_through_value_provider()
        {
            var valueProvider = Substitute.For<IValueProvider>();
            valueProvider.GetValue().Returns("1234");

            var entity = new Entity();
            var accessor = new EntityAccessor<Entity>(entity);
            accessor.Set("xts_attribute", valueProvider);

            Assert.Equal("1234", entity.Get<string>("xts_attribute"));
        }

        [Fact]
        public void Can_set_through_value_provider_generic()
        {
            var valueProvider = Substitute.For<IValueProvider<string>>();
            valueProvider.GetValue().Returns("1234");

            var entity = new xts_entity { Id = Guid.NewGuid() };
            var accessor = new EntityAccessor<xts_entity>(entity);
            accessor.Set(e => e.xts_string, valueProvider);

            Assert.Equal("1234", entity.Get(e => e.xts_string));
        }

        [Fact]
        public void Can_use_equal_expression()
        {
            var id = Guid.NewGuid();
            var reference = new EntityReference("xts_reference", id);
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_referenceid, reference);
            var accessor = new EntityAccessor<xts_entity>(entity);

            var comparisonValue = new EntityReference("xts_reference", id);
            Assert.True(accessor.Equal(e => e.xts_referenceid, comparisonValue));
            Assert.False(accessor.Equal(e => e.xts_referenceid, new EntityReference("xts_reference", Guid.NewGuid())));
        }

        [Fact]
        public void Can_use_equal_string_attribute_name()
        {
            var id = Guid.NewGuid();
            var reference = new EntityReference("xts_reference", id);
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_referenceid, reference);
            var accessor = new EntityAccessor<xts_entity>(entity);

            var comparisonValue = new EntityReference("xts_reference", id);
            Assert.True(accessor.Equal("xts_referenceid", comparisonValue));
            Assert.False(accessor.Equal("xts_referenceid", new EntityReference("xts_reference", Guid.NewGuid())));
        }

        private enum TestOptions
        {
            SomeValue = 47
        }

        [Fact] // Assume ActivityParty
        public void Can_set_entity_collection_using_single_entity_reference()
        {
            var entityReference = new EntityReference("equipment", Guid.NewGuid());
            var entity = new xts_entity();
            var accessor = new EntityAccessor<xts_entity>(entity);
            accessor.Set(e => e.xts_activityparties, entityReference);

            var collection = accessor.Get(e => e.xts_activityparties);
            var activityParty = collection.Entities[0].ToEntity<ActivityParty>();
            Assert.Equal(entityReference, activityParty.Get(e => e.PartyId));
        }

        [Fact] // Assume ActivityParty
        public void Can_set_entity_collection_using_params_entity_reference()
        {
            var equipmentReference = new EntityReference("equipment", Guid.NewGuid());
            var leadReference = new EntityReference("lead", Guid.NewGuid());
            var entity = new xts_entity();
            var accessor = new EntityAccessor<xts_entity>(entity);
            accessor.Set(e => e.xts_activityparties, equipmentReference, leadReference);

            var collection = accessor.Get(e => e.xts_activityparties);
            Assert.Equal(equipmentReference, collection.Entities[0].ToEntity<ActivityParty>().Get(e => e.PartyId));
            Assert.Equal(leadReference, collection.Entities[1].ToEntity<ActivityParty>().Get(e => e.PartyId));
        }

        [Fact] // Assume ActivityParty
        public void Can_set_entity_collection_using_entity_references()
        {
            var equipmentReference = new EntityReference("equipment", Guid.NewGuid());
            var leadReference = new EntityReference("lead", Guid.NewGuid());
            var references = new[] { equipmentReference, leadReference };
            var entity = new xts_entity();
            var accessor = new EntityAccessor<xts_entity>(entity);
            accessor.Set(e => e.xts_activityparties, references);

            var collection = accessor.Get(e => e.xts_activityparties);
            Assert.Equal(equipmentReference, collection.Entities[0].ToEntity<ActivityParty>().Get(e => e.PartyId));
            Assert.Equal(leadReference, collection.Entities[1].ToEntity<ActivityParty>().Get(e => e.PartyId));
        }
    }
}

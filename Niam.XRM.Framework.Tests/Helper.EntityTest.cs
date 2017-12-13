using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using NSubstitute;
using Niam.XRM.Framework.Interfaces;
using Xunit;

namespace Niam.XRM.Framework.Tests
{
    public class HelperEntityTest
    {
        [Fact]
        public void Can_convert_entity_to_entity_accessor()
        {
            var entity = new Entity("xts_entity", Guid.NewGuid());
            var accessor = entity.ToEntityAccessor<xts_entity>();
            Assert.Equal(entity.ToEntityReference(), accessor.Entity.ToEntityReference());

            var earlyBoundEntity = new xts_entity { Id = Guid.NewGuid() };
            var earlyBoundAccessor = earlyBoundEntity.ToEntityAccessor();
            Assert.Equal(earlyBoundEntity.ToEntityReference(), earlyBoundAccessor.Entity.ToEntityReference());
        }

        [Fact]
        public void Can_get_entity_name()
        {
            Assert.Equal("xts_entity", Helper.Name<xts_entity>());
        }

        [Fact]
        public void Can_get_attribute_name_from_custom_entity()
        {
            var entity = new xts_entity();
            Assert.Equal("xts_entityid", Helper.Name<xts_entity>(e => e.Id));
            Assert.Equal("xts_entityid", entity.Name(e => e.Id));
            Assert.Equal("xts_column", Helper.Name<xts_entity>(e => e.xts_withcolumnattribute));
            Assert.Equal("xts_column", entity.Name(e => e.xts_withcolumnattribute));
            Assert.Equal("attributewithcasechar", Helper.Name<xts_entity>(e => e.AttributeWithCaseChar));
            Assert.Equal("attributewithcasechar", entity.Name(e => e.AttributeWithCaseChar));
        }

        [Fact]
        public void Can_get_attribute_name_from_crmsvcutil_generated()
        {
            var account = new MsGenerated.Account();
            Assert.Equal("accountid", Helper.Name<MsGenerated.Account>(e => e.Id));
            Assert.Equal("accountid", account.Name(e => e.Id));
        }

        [Fact]
        public void Can_get_info_for_entity_type()
        {
            Assert.False(Helper.Info<Entity>().IsCrmSvcUtilGenerated);
            Assert.Equal("CRM_SDK_ENTITY", Helper.Info<Entity>().LogicalName);
            Assert.Null(Helper.Info<Entity>().PrimaryNameAttribute);
            Assert.Null(Helper.Info<Entity>().StateCodeActiveValue);
            Assert.Null(Helper.Info<Entity>().GetAttributeName("hello"));
            Assert.Null(Helper.Info<Entity>().GetMemberName("world"));
        }

        [Fact]
        public void ContainsAny_one_criteria()
        {
            var entity = new Entity("entity") { Id = Guid.NewGuid() };
            entity["xts_attr"] = null;

            Assert.False(entity.ContainsAny("xts_unknown"));
            Assert.True(entity.ContainsAny("xts_attr"));
        }

        [Theory]
        [MemberData(nameof(GetContainsAnyScenarios))]
        public void ContainsAny_many_criterias(string[] contains, bool expected)
        {
            var entity = new Entity("entity") { Id = Guid.NewGuid() };
            entity["xts_attr"] = null;
            var actual = entity.ContainsAny(contains.First(), contains.Skip(1).ToArray());
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> GetContainsAnyScenarios()
        {
            yield return new object[] { new[] { "xts_unknown" }, false };
            yield return new object[] { new[] { "xts_attr", "xts_unknown" }, true };
            yield return new object[] { new[] { "xts_unknown", "xts_attr" }, true };
        }

        [Fact]
        public void ContainsAny_generic_one_criteria()
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity["xts_attribute"] = null;

            Assert.False(entity.ContainsAny(e => e.AttributeWithCaseChar));
            Assert.True(entity.ContainsAny(e => e.xts_attribute));
        }

        [Theory]
        [MemberData(nameof(GetContainsAnyGenericScenarios))]
        public void ContainsAny_generic_many_criterias(Expression<Func<xts_entity, object>>[] contains, bool expected)
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity["xts_attribute"] = null;
            var actual = entity.ContainsAny(contains.First(), contains.Skip(1).ToArray());
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> GetContainsAnyGenericScenarios()
        {
            yield return new object[]
            {
                new Expression<Func<xts_entity, object>>[]
                {
                    e => e.xts_withcolumnattribute
                },
                false
            };

            yield return new object[]
            {
                new Expression<Func<xts_entity, object>>[]
                {
                    e => e.xts_attribute,
                    e => e.AttributeWithCaseChar
                },
                true
            };

            yield return new object[]
            {
                new Expression<Func<xts_entity, object>>[]
                {
                    e => e.AttributeWithCaseChar,
                    e => e.xts_attribute
                },
                true
            };
        }

        [Fact]
        public void Can_get_attribute_value()
        {
            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            var reference = new EntityReference("reference", Guid.NewGuid());
            entity["xts_attribute"] = reference;

            Assert.Equal(reference, entity.Get<EntityReference>("xts_attribute"));
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

            Assert.Equal(reference, entity.Get(e => e.xts_attribute));
        }

        [Fact]
        public void Can_get_money_value()
        {
            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            entity["xts_money"] = null;

            Assert.Equal(0m, entity.GetValue(e => e.xts_money));
            Assert.Equal(10m, entity.GetValue(e => e.xts_money, 10m));

            entity["xts_money"] = new Money(250m);
            Assert.Equal(250m, entity.GetValue(e => e.xts_money, 250m));
        }

        [Fact]
        public void Can_get_formatted_value()
        {
            var entity = new Entity();
            entity.FormattedValues["xts_attribute"] = "formatted-value-1234";
            Assert.Equal("formatted-value-1234", entity.GetFormattedValue("xts_attribute"));
            Assert.Null(entity.GetFormattedValue("xts_notexist"));
        }

        [Fact]
        public void Can_get_formatted_value_generic()
        {
            var entity = new xts_entity();
            entity.FormattedValues["xts_optionsetvalue"] = "formatted-value-4567";
            Assert.Equal("formatted-value-4567", entity.GetFormattedValue(e => e.xts_optionsetvalue));
            Assert.Null(entity.GetFormattedValue(e => e.xts_decimal));
        }

        [Fact]
        public void Can_set_formatted_value()
        {
            var entity = new Entity();
            entity.SetFormattedValue("xts_attribute", "formatted-value-1234");
            Assert.Equal("formatted-value-1234", entity.FormattedValues["xts_attribute"]);
        }

        [Fact]
        public void Can_set_formatted_value_generic()
        {
            var entity = new xts_entity();
            entity.SetFormattedValue(e => e.xts_optionsetvalue, "formatted-value-4567");
            Assert.Equal("formatted-value-4567", entity.FormattedValues["xts_optionsetvalue"]);
        }

        [Fact]
        public void Can_set_primary_field_generic()
        {
            var id = Guid.NewGuid();
            var testEntity = new new_testentity
            {
                Id = id
            };
            
            Assert.Equal(id, testEntity.Id);
            Assert.Equal(id, testEntity.Get(e => e.new_testentityId));

            testEntity = new new_testentity();
            testEntity.Set(e => e.new_testentityId, id);
            Assert.Equal(id, testEntity.Id);
            Assert.Equal(id, testEntity.Get(e => e.new_testentityId));
        }

        [Fact]
        public void Can_set_primary_field()
        {
            var id = Guid.NewGuid();
            var testEntity = new new_testentity();
            testEntity.Set("new_testentityid", id);
            Assert.Equal(id, testEntity.Id);
            Assert.Equal(id, testEntity.Get(e => e.new_testentityId));
        }

        [Fact]
        public void Can_set_attribute_value()
        {
            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            var reference = new EntityReference("reference", Guid.NewGuid());
            entity.Set("xts_attribute", reference);

            Assert.Equal(reference, entity["xts_attribute"]);
        }

        [Fact]
        public void Can_set_attribute_value_generic()
        {
            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            var reference = new EntityReference("reference", Guid.NewGuid());
            entity.Set(e => e.xts_attribute, reference);
            
            Assert.Equal(reference, entity["xts_attribute"]);
        }

        [Fact]
        public void Can_set_option_set_value_using_int()
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_optionsetvalue, 12);

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
            entity.Set(e => e.xts_optionsetvalue, TestOptions.SomeValue);

            Assert.Equal(47, entity.Get(e => e.xts_optionsetvalue).Value);
        }

        [Fact]
        public void Can_set_money_using_decimal()
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_money, 1234);

            Assert.Equal(1234m, entity.Get(e => e.xts_money).Value);
        }

        [Fact]
        public void Can_set_through_attribute_value_provider()
        {
            var valueProvider = Substitute.For<IAttributeValueProvider>();
            valueProvider.GetValueFor(Arg.Any<string>()).Returns("1234");

            var entity = new Entity();
            entity.Set("xts_attribute", valueProvider);

            Assert.Equal("1234", entity.Get<string>("xts_attribute"));
        }

        [Fact]
        public void Can_set_through_attribute_value_provider_generic()
        {
            var valueProvider = Substitute.For<IAttributeValueProvider<xts_entity, string>>();
            valueProvider.GetValueFor(Arg.Any<Expression<Func<xts_entity, string>>>()).Returns("1234");

            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_string, valueProvider);

            Assert.Equal("1234", entity.Get(e => e.xts_string));
        }

        [Fact]
        public void Can_set_through_value_provider()
        {
            var valueProvider = Substitute.For<IValueProvider>();
            valueProvider.GetValue().Returns("1234");

            var entity = new Entity();
            entity.Set("xts_attribute", valueProvider);

            Assert.Equal("1234", entity.Get<string>("xts_attribute"));
        }
        
        [Fact]
        public void Can_set_through_value_provider_generic()
        {
            var valueProvider = Substitute.For<IValueProvider<string>>();
            valueProvider.GetValue().Returns("1234");

            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_string, valueProvider);

            Assert.Equal("1234", entity.Get(e => e.xts_string));
        }

        [Fact]
        public void Can_set_nullable_value_type_through_value_provider_generic_value_type()
        {
            var intValueProvider = Substitute.For<IValueProvider<int>>();
            intValueProvider.GetValue().Returns(1234);
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_int, intValueProvider);

            Assert.Equal(1234, entity.Get(e => e.xts_int));

            var date = new DateTime(2017, 3, 2);
            var dateValueProvider = Substitute.For<IValueProvider<DateTime>>();
            dateValueProvider.GetValue().Returns(date);
            entity.Set(e => e.xts_datetime, dateValueProvider);

            Assert.Equal(date, entity.Get(e => e.xts_datetime));
        }

        [Fact]
        public void Can_set_money_from_decimal_value_provider()
        {
            var decimalValueProvider = Substitute.For<IValueProvider<decimal>>();
            decimalValueProvider.GetValue().Returns(1500m);

            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_money, decimalValueProvider);

            Assert.Equal(1500m, entity.Get(e => e.xts_money).Value);
        }

        [Fact]
        public void Can_remove_attribute()
        {
            var entity = new Entity("entity")
            {
                ["xts_attribute"] = 1234
            };

            Assert.True(entity.Contains("xts_attribute"));
            entity.Remove("xts_attribute");
            Assert.False(entity.Contains("xts_attribute"));
        }

        [Fact]
        public void Can_remove_attribute_generic()
        {
            var entity = new xts_entity();
            entity.Set(e => e.xts_decimal, 1234);

            Assert.True(entity.Contains("xts_decimal"));
            entity.Remove(e => e.xts_decimal);
            Assert.False(entity.Contains("xts_decimal"));
        }
        
        [Fact]
        public void Can_check_whether_entity_is_crmsvcutil_generated_or_not()
        {
            Assert.False(Helper.Info<xts_entity>().IsCrmSvcUtilGenerated);
            Assert.True(Helper.Info<CrmSvcUtilGeneratedEntity>().IsCrmSvcUtilGenerated);
        }

        [Fact]
        public void MsGenerated_set_through_property()
        {
            var entity = new CrmSvcUtilGeneratedEntity();
            entity.Set(e => e.AttributeThroughProperty, new OptionSetValue(1));
            Assert.True(entity.SetThroughProperty);
        }

        [EntityLogicalName("CrmSvcUtilGeneratedEntity")]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "8.1.0.239")]
        private class CrmSvcUtilGeneratedEntity : Entity
        {
            public bool SetThroughProperty { get; private set; }

            [AttributeLogicalNameAttribute("attributethroughproperty")]
            public OptionSetValue AttributeThroughProperty
            {
                get => GetAttributeValue<OptionSetValue>("attributethroughproperty");
                set
                {
                    SetAttributeValue("attributethroughproperty", value);
                    SetThroughProperty = true;
                }
            }
        }

        [Fact]
        public void Can_get_value_nullable_type()
        {
            var entity = new xts_entity();

            Assert.Equal(0, entity.GetValue(e => e.xts_int));
            Assert.Equal(111, entity.GetValue(e => e.xts_int, 111));
            entity.Set(e => e.xts_int, 123);
            Assert.Equal(123, entity.GetValue(e => e.xts_int));
            Assert.Equal(123, entity.GetValue(e => e.xts_int, 3333));

            Assert.Equal(0m, entity.GetValue(e => e.xts_decimal));
            Assert.Equal(1000m, entity.GetValue(e => e.xts_decimal, 1000m));
            entity.Set(e => e.xts_decimal, 2300m);
            Assert.Equal(2300m, entity.GetValue(e => e.xts_decimal));
            Assert.Equal(2300m, entity.GetValue(e => e.xts_decimal, 4000m));
        }

        [Fact]
        public void Can_get_primary_attribute()
        {
            Assert.Equal("primarynameattributekey", Helper.PrimaryAttribute<xts_keytest>());
            Assert.Null(Helper.PrimaryAttribute<xts_nokeytest>());
        }

        [Fact]
        public void Can_get_active_status_value()
        {
            Assert.Equal(12, Helper.Info<xts_activestatecodetest>().StateCodeActiveValue);
            Assert.Null(Helper.Info<xts_nokeytest>().StateCodeActiveValue);
        }

        [Fact] // Assume ActivityParty
        public void Can_set_entity_collection_using_single_entity_reference()
        {
            var entityReference = new EntityReference("equipment", Guid.NewGuid());
            var entity = new xts_entity();
            entity.Set(e => e.xts_activityparties, entityReference);

            var collection = entity.Get(e => e.xts_activityparties);
            var activityParty = collection.Entities[0].ToEntity<ActivityParty>();
            Assert.Equal(entityReference, activityParty.Get(e => e.PartyId));
        }

        [Fact] // Assume ActivityParty
        public void Can_set_entity_collection_using_params_entity_reference()
        {
            var equipmentReference = new EntityReference("equipment", Guid.NewGuid());
            var leadReference = new EntityReference("lead", Guid.NewGuid());
            var entity = new xts_entity();
            entity.Set(e => e.xts_activityparties, equipmentReference, leadReference);

            var collection = entity.Get(e => e.xts_activityparties);
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
            entity.Set(e => e.xts_activityparties, references);

            var collection = entity.Get(e => e.xts_activityparties);
            Assert.Equal(equipmentReference, collection.Entities[0].ToEntity<ActivityParty>().Get(e => e.PartyId));
            Assert.Equal(leadReference, collection.Entities[1].ToEntity<ActivityParty>().Get(e => e.PartyId));
        }
    }
}

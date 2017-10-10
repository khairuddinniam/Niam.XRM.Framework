using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using NSubstitute;
using Niam.XRM.Framework.Plugin;
using Xunit;

namespace Niam.XRM.Framework.Tests
{
    public class HelperTest
    {
        [Fact]
        public void IOrganizationService_invalid_when_delete_null_entity_reference()
        {
            var service = Substitute.For<IOrganizationService>();
            var ex = Assert.Throws<ArgumentNullException>(() => service.Delete(null));
            Assert.Equal("reference", ex.ParamName);
        }

        [Fact]
        public void IOrganizationService_can_delete_entity_reference()
        {
            EntityReference deletedReference = null;
            var service = Substitute.For<IOrganizationService>();
            service.When(svc => svc.Delete(Arg.Any<string>(), Arg.Any<Guid>()))
                .Do(ci =>
                {
                    var logicalName = ci.ArgAt<string>(0);
                    var id = ci.ArgAt<Guid>(1);
                    deletedReference = new EntityReference(logicalName, id);
                });

            var reference = new EntityReference("entity", Guid.NewGuid());
            service.Delete(reference);
            Assert.Equal(reference, deletedReference);
        }

        [Fact]
        public void PluginExecutionContext_get_request()
        {
            var inputs = new ParameterCollection();
            var context = Substitute.For<IPluginExecutionContext>();
            context.InputParameters.Returns(inputs);

            var createEntity = new Entity("create") { Id = Guid.NewGuid() };
            inputs["Target"] = createEntity;
            Assert.Equal(createEntity, context.GetRequest<CreateRequest>().Target);

            var updateEntity = new Entity("update") { Id = Guid.NewGuid() };
            inputs["Target"] = updateEntity;
            Assert.Equal(updateEntity, context.GetRequest<UpdateRequest>().Target);

            var deleteReference = new EntityReference("delete", Guid.NewGuid());
            inputs["Target"] = deleteReference;
            Assert.Equal(deleteReference, context.GetRequest<DeleteRequest>().Target);
        }

        [Fact]
        public void Can_get_image_from_image_collections()
        {
            var entityImage = new Entity("image") { Id = Guid.NewGuid() };
            var images = new EntityImageCollection();
            images["IMAGE_KEY"] = entityImage;

            Assert.Equal(entityImage, images.GetImage("IMAGE_KEY"));
            Assert.Null(images.GetImage("UNKNOWN_KEY"));
        }

        [Fact]
        public void EntityReference_to_entity()
        {
            var reference = new EntityReference("entity", Guid.NewGuid());
            var entity = reference.ToEntity();
            Assert.Equal(reference.LogicalName, entity.LogicalName);
            Assert.Equal(reference.Id, entity.Id);
        }

        [Fact]
        public void Money_get_value_or_default()
        {
            Assert.Equal(0m, ((Money)null).GetValueOrDefault());
            Assert.Equal(1000m, ((Money)null).GetValueOrDefault(1000m));
            Assert.Equal(1500m, new Money(1500m).GetValueOrDefault());
            Assert.Equal(2000m, new Money(2000m).GetValueOrDefault(3500m));
        }

        [Fact]
        public void TransactionContextEntity_ToEntityReference()
        {
            var entity = new Entity("entityname", Guid.NewGuid());
            var txEntity = new TransactionContextEntity<Entity>(entity);
            Assert.Equal(entity.ToEntityReference(), txEntity.ToEntityReference());
        }

        [Fact]
        public void TransactionContextEntityGeneric_ToEntityReference()
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };
            var txEntity = new TransactionContextEntity<xts_entity>(entity);
            Assert.Equal(entity.ToEntityReference(), txEntity.ToEntityReference());
        }

        [Fact]
        public void Can_box_expression()
        {
            Expression<Func<xts_entity, DateTime?>> dateTimeExpression = e => e.Get(x => x.xts_datetime);
            var boxedExpression = Helper.BoxExpression(dateTimeExpression);

            var dateTime = DateTime.UtcNow;
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_datetime, dateTime);

            Assert.Equal(dateTime, boxedExpression.Compile().Invoke(entity));
        }

        [Fact]
        public void Can_do_equality_value_is_equal()
        {
            Assert.True(Helper.Equal((EntityReference) null, null));
            Assert.True(Helper.Equal((OptionSetValue) null, (OptionSetValue) null));
            Assert.True(Helper.Equal((Money) null, null));
            Assert.True(Helper.Equal((DateTime?) null, null));
            Assert.True(Helper.Equal((string) null, null));
            Assert.True(Helper.Equal((int?) null, null));
            Assert.True(Helper.Equal((float?) null, null));
            Assert.True(Helper.Equal((decimal?) null, null));

            var entityReference = new EntityReference("xts_reference", Guid.NewGuid());
            var optionSetValue = new OptionSetValue(1234);
            var money = new Money(1500m);
            DateTime? date = new DateTime(2017, 1, 2);
            var text = "text";
            int? intNumber = 12;
            float? floatNumber = 15f;
            decimal? decimalNumber = 460m;

            Assert.True(Helper.Equal(entityReference, new EntityReference("xts_reference", entityReference.Id)));
            Assert.True(Helper.Equal(optionSetValue, new OptionSetValue(1234)));
            Assert.True(Helper.Equal(money, new Money(1500m)));
            Assert.True(Helper.Equal(date, new DateTime(2017, 1, 2)));
            Assert.True(Helper.Equal(text, "text"));
            Assert.True(Helper.Equal(intNumber, 12));
            Assert.True(Helper.Equal(floatNumber, 15));
            Assert.True(Helper.Equal(decimalNumber, 460));
        }

        [Fact]
        public void Can_do_equality_value_is_not_equal()
        {
            var entityReference = new EntityReference("xts_reference", Guid.NewGuid());
            var optionSetValue = new OptionSetValue(1234);
            var money = new Money(1500m);
            DateTime? date = new DateTime(2017, 1, 2);
            var text = "text";
            int? intNumber = 12;
            float? floatNumber = 15f;
            decimal? decimalNumber = 460m;

            Assert.False(Helper.Equal(null, entityReference));
            Assert.False(Helper.Equal(null, optionSetValue));
            Assert.False(Helper.Equal(null, money));
            Assert.False(Helper.Equal(null, date));
            Assert.False(Helper.Equal(null, text));
            Assert.False(Helper.Equal(null, intNumber));
            Assert.False(Helper.Equal(null, floatNumber));
            Assert.False(Helper.Equal(null, decimalNumber));
            
            Assert.False(Helper.Equal(entityReference, null));
            Assert.False(Helper.Equal(optionSetValue, (OptionSetValue) null));
            Assert.False(Helper.Equal(money, null));
            Assert.False(Helper.Equal(date, null));
            Assert.False(Helper.Equal(text, null));
            Assert.False(Helper.Equal(intNumber, null));
            Assert.False(Helper.Equal(floatNumber, null));
            Assert.False(Helper.Equal(decimalNumber, null));
        }

        enum EqualityOptions
        {
            First = 1,
            Second = 2
        }

        [Fact]
        public void Can_do_equality_option_set_value_to_enum()
        {
            Assert.True(Helper.Equal(new OptionSetValue(1), EqualityOptions.First));
            Assert.False(Helper.Equal(new OptionSetValue(1), EqualityOptions.Second));
            Assert.False(Helper.Equal(null, EqualityOptions.First));
            Assert.False(Helper.Equal(null, EqualityOptions.Second));
            EqualityOptions? nullEnum = null;
            Assert.False(Helper.Equal(new OptionSetValue(1), nullEnum));
            Assert.True(Helper.Equal((OptionSetValue) null, nullEnum));
        }

        [Fact]
        public void Can_do_equality_option_set_equals_any()
        {
            var optionSet = new OptionSetValue(2);
            Assert.False(Helper.EqualsAny(optionSet, EqualityOptions.First));
            Assert.True(Helper.EqualsAny(optionSet, EqualityOptions.Second));
            Assert.True(Helper.EqualsAny(optionSet, EqualityOptions.First, EqualityOptions.Second));

            optionSet = new OptionSetValue(3);
            Assert.False(Helper.EqualsAny(optionSet, EqualityOptions.First, EqualityOptions.Second));

            OptionSetValue nullOption = null;
            EqualityOptions? nullEnum = null;

            Assert.True(Helper.EqualsAny(nullOption, EqualityOptions.First, nullEnum, EqualityOptions.Second));
        }

        [Fact]
        public void Can_full_outer_join()
        {
            var left = new int?[] { 1, 2, 3 };
            var right = new int?[] { 2, 3, 4 };

            var joinResults = left.FullOuterJoin(right, l => l, r => r, (l, r, key) => new { Left = l, Right = r });
            var expected = new[]
            {
                new { Left = (int?) 1, Right = (int?) null },
                new { Left = (int?) 2, Right = (int?) 2 },
                new { Left = (int?) 3, Right = (int?) 3 },
                new { Left = (int?) null, Right = (int?) 4 }
            };
            Assert.Equal(expected, joinResults);
        }

        [Fact]
        public void Can_get_or_add_dictionary_storage_using_value()
        {
            var value = new Entity();
            var storage = new Dictionary<string, object>();
            Assert.Same(value, storage.GetOrAdd("key", value));
            Assert.True(storage.ContainsKey("key"));
            Assert.Same(value, storage.GetOrAdd("key", value));
        }

        [Fact]
        public void Can_get_or_add_dictionary_storage_using_value_factory()
        {
            var called = 0;
            var value = new Entity();

            Entity ValueFactory()
            {
                called++;
                return value;
            }

            var storage = new Dictionary<string, object>();
            Assert.Same(value, storage.GetOrAdd("key", ValueFactory));
            Assert.True(storage.ContainsKey("key"));
            Assert.Equal(1, called);
            Assert.Same(value, storage.GetOrAdd("key", ValueFactory));
            Assert.Equal(1, called);
        }

        public class JsonData
        {
            public Guid RecordId { get; set; }
        }

        [Fact]
        public void Can_serialize_to_json()
        {
            var data = new JsonData
            {
                RecordId = Guid.NewGuid()
            };

            var json = data.ToJson();
            Assert.Equal("{\"RecordId\":\"" + data.RecordId.ToString("d") + "\"}", json);
        }

        [Fact]
        public void Can_deserialize_from_json()
        {
            var recordId = Guid.NewGuid();
            var json = "{\"RecordId\":\"" + recordId.ToString("d") + "\"}";
            var data = json.FromJson<JsonData>();
            Assert.Equal(recordId, data.RecordId);
        }
    }
}

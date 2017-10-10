using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Interfaces.Plugin;
using Xunit;

namespace Niam.XRM.TestFramework.Tests
{
    public class TestHelperTest
    {
        [Fact]
        public void Can_create_transaction_context()
        {
            var testHelper = new TestHelper();
            var context = testHelper.CreateTransactionContext<xts_entity>();
            Assert.Equal(context.Reference.Entity.ToEntityReference(), context.Input.Entity.ToEntityReference());

            ITransactionContext<Entity> baseContext = context;
            Assert.Equal(baseContext.Reference.Entity.ToEntityReference(), baseContext.Input.Entity.ToEntityReference());

            context.Input.Set(e => e.xts_money, new Money(200m));
            Assert.Equal(200m, context.Reference.Get(e => e.xts_money).Value);
        }

        [Fact]
        public void Can_use_shared_variables()
        {
            var testHelper = new TestHelper();
            var context = testHelper.CreateTransactionContext<xts_entity>();
            context.PluginExecutionContext.SharedVariables["my-shared"] = 1234;
            Assert.Equal(1234, context.PluginExecutionContext.SharedVariables["my-shared"]);
        }

        [Fact]
        public void Can_use_session_storage()
        {
            var testHelper = new TestHelper();
            var context = testHelper.CreateTransactionContext<xts_entity>();
            context.SessionStorage["session-storage"] = 5678;
            Assert.Equal(5678, context.SessionStorage["session-storage"]);
        }

        [Fact]
        public void Can_create_record_no_set_id()
        {
            var testHelper = new TestHelper();
            var createEntity = new xts_entity();
            createEntity.Set(e => e.xts_money, new Money(400m));
            var createdId = testHelper.Service.Create(createEntity);
            createEntity.Id = createdId;

            var dbCreatedEntity = testHelper.Db.First().Value.ToEntity<xts_entity>();
            Assert.Equal(createdId, dbCreatedEntity.Id);
            Assert.Equal(createEntity.ToEntityReference(), dbCreatedEntity.ToEntityReference());
            Assert.NotSame(createEntity, dbCreatedEntity);

            createEntity.Get(e => e.xts_money).Value = 300m;
            Assert.Equal(400m, dbCreatedEntity.Get(e => e.xts_money).Value);
        }

        [Fact]
        public void Can_create_record_previously_set_id()
        {
            var testHelper = new TestHelper();
            var createEntity = new xts_entity { Id = Guid.NewGuid() };
            createEntity.Set(e => e.xts_money, new Money(400m));
            var createdId = testHelper.Service.Create(createEntity);

            var dbCreatedEntity = testHelper.Db.First().Value.ToEntity<xts_entity>();
            Assert.Equal(createdId, dbCreatedEntity.Id);
            Assert.Equal(createEntity.ToEntityReference(), dbCreatedEntity.ToEntityReference());
            Assert.NotSame(createEntity, dbCreatedEntity);

            createEntity.Get(e => e.xts_money).Value = 300m;
            Assert.Equal(400m, dbCreatedEntity.Get(e => e.xts_money).Value);
        }

        [Fact]
        public void Invalid_when_created_null_entity()
        {
            var testHelper = new TestHelper();
            var ex = Assert.Throws<InvalidOperationException>(() => testHelper.Service.Create(null));
            Assert.Equal("Parameter 'entity' is null.", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Invalid_when_create_entity_with_null_or_empty_logical_name(string logicalName)
        {
            var testHelper = new TestHelper();
            var entity = new Entity { LogicalName = logicalName };
            var ex = Assert.Throws<InvalidOperationException>(() => testHelper.Service.Create(entity));
            Assert.Equal("Parameter 'entity.LogicalName' is null or empty.", ex.Message);
        }

        [Fact]
        public void Can_update_record_with_no_record_in_db()
        {
            var testHelper = new TestHelper();
            var source = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            source.Set(e => e.xts_money, new Money(400m));

            testHelper.Service.Update(source);
            var updatedEntity = testHelper.UpdatedEntities.First().ToEntity<xts_entity>();

            Assert.False(testHelper.Db.Any());
            Assert.Equal(source.ToEntityReference(), updatedEntity.ToEntityReference());
            Assert.Equal(400m, updatedEntity.Get(e => e.xts_money).Value);

            source.Get(e => e.xts_money).Value = 300m;
            Assert.Equal(400m, updatedEntity.Get(e => e.xts_money).Value);
            Assert.NotSame(source, updatedEntity);
        }

        [Fact]
        public void Can_update_record_with_have_record_in_db()
        {
            var testHelper = new TestHelper();
            var id = Guid.NewGuid();
            var dbEntity = new xts_entity
            {
                Id = id
            };
            dbEntity.Set(e => e.xts_withcolumnattribute, "TEST");
            dbEntity.Set(e => e.xts_money, new Money(200m));
            testHelper.Db["DATA"] = dbEntity;

            var source = new xts_entity
            {
                Id = id
            };
            source.Set(e => e.xts_money, new Money(400m));

            testHelper.Service.Update(source);
            var updatedEntity = testHelper.UpdatedEntities.First().ToEntity<xts_entity>();

            Assert.True(testHelper.Db.Count == 1);
            Assert.Equal("TEST", dbEntity.Get(e => e.xts_withcolumnattribute));
            Assert.Equal(400m, dbEntity.Get(e => e.xts_money).Value);

            Assert.Equal(source.ToEntityReference(), updatedEntity.ToEntityReference());
            Assert.Equal(400m, updatedEntity.Get(e => e.xts_money).Value);
            Assert.NotSame(source, updatedEntity);

            source.Get(e => e.xts_money).Value = 300m;
            Assert.Equal(400m, updatedEntity.Get(e => e.xts_money).Value);
        }

        [Fact]
        public void Can_delete_record_with_no_record_in_db()
        {
            var testHelper = new TestHelper();
            var logicalName = "entity";
            var id = Guid.NewGuid();
            testHelper.Service.Delete(logicalName, id);

            var deletedReference = testHelper.DeletedEntities.First();
            Assert.Equal(logicalName, deletedReference.LogicalName);
            Assert.Equal(id, deletedReference.Id);
            Assert.False(testHelper.Db.Any());
        }

        [Fact]
        public void Can_delete_record_with_have_record_in_db()
        {
            var testHelper = new TestHelper();
            var logicalName = "entity";
            var id = Guid.NewGuid();

            var entity = new Entity(logicalName, id);
            testHelper.Db["data"] = entity;
            Assert.True(testHelper.Db.Any());

            testHelper.Service.Delete(logicalName, id);

            var deletedReference = testHelper.DeletedEntities.First();
            Assert.Equal(logicalName, deletedReference.LogicalName);
            Assert.Equal(id, deletedReference.Id);
            Assert.False(testHelper.Db.Any());
        }

        [Fact]
        public void Can_retrieve_record()
        {
            var testHelper = new TestHelper();
            var logicalName = "entity";
            var id = Guid.NewGuid();

            var reference = new EntityReference("reference", Guid.NewGuid());
            var entity = new Entity(logicalName, id)
            {
                ["xts_text"] = "text-value",
                ["xts_money"] = new Money(100m),
                ["xts_referenceid"] = reference,
                ["xts_optionsetvalue"] = new OptionSetValue(1)
            };
            testHelper.Db["data"] = entity;
            Assert.True(testHelper.Db.Any());

            var result = testHelper.Service.Retrieve(logicalName, id, null);
            Assert.NotSame(entity, result);
            Assert.Equal(result.ToEntityReference(), entity.ToEntityReference());
            Assert.False(result.Contains("xts_text"));
            Assert.False(result.Contains("xts_money"));
            Assert.False(result.Contains("xts_referenceid"));
            Assert.False(result.Contains("xts_optionsetvalue"));

            result = testHelper.Service.Retrieve(logicalName, id, new ColumnSet("xts_text", "xts_referenceid"));
            Assert.NotSame(entity, result);
            Assert.Equal(result.ToEntityReference(), entity.ToEntityReference());
            Assert.Equal("text-value", result["xts_text"]);
            Assert.Equal(reference, result["xts_referenceid"]);

            result = testHelper.Service.Retrieve(logicalName, id, new ColumnSet(true));
            Assert.NotSame(entity, result);
            Assert.Equal(result.ToEntityReference(), entity.ToEntityReference());
            Assert.Equal("text-value", result["xts_text"]);
            Assert.Equal(new Money(100m), result["xts_money"]);
            Assert.Equal(reference, result["xts_referenceid"]);
            Assert.Equal(new OptionSetValue(1), result["xts_optionsetvalue"]);
        }
    }
}

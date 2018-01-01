using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin;
using Xunit;

namespace Niam.XRM.TestFramework.Tests
{
    public class TestBaseTest : TestBase<xts_entity>
    {
        protected override void Configure(IPluginConfiguration<xts_entity> config)
        {
            config.ColumnSet = new ColumnSet<xts_entity>(
                e => e.xts_withcolumnattribute,
                e => e.xts_money
            );
        }

        [Fact]
        public void Can_update_reference()
        {
            var reference = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            reference.Set(e => e.xts_withcolumnattribute, "test1234");
            reference.Set(e => e.xts_money, new Money(100m));
            Reference = reference;

            Assert.Equal(Input.ToEntityReference(), reference.ToEntityReference());
            Assert.Equal(Context.Input.ToEntityReference(), Context.Reference.ToEntityReference());
            Assert.Equal(reference.Id, Context.Reference.Entity.Id);
            Assert.Equal(reference.Get(e => e.xts_withcolumnattribute), Context.Reference.Entity.Get(e => e.xts_withcolumnattribute));
            Assert.Equal(reference.Get(e => e.xts_money), Context.Reference.Entity.Get(e => e.xts_money));
        }

        [Fact]
        public void Can_execute_operation()
        {
            var reference = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            Reference = reference;

            new Operation(Context).Execute();
            Assert.Equal(100m, Input.GetValue(e => e.xts_money));
            Assert.Equal("Hello", Input.Get(e => e.xts_withcolumnattribute));
        }

        [Fact]
        public void Can_access_reference_properties()
        {
            var relationship = new Relationship();
            var relatedEntities = new EntityCollection();

            var reference = new xts_entity
            {
                Id = Guid.NewGuid(),
                EntityState = EntityState.Changed,
                RowVersion = "row-version",
                FormattedValues =
                {
                    ["xts_attribute"] = "Hello World"
                },
                KeyAttributes =
                {
                    ["xts_keyattribute"] = "my-key-1234"
                },
                RelatedEntities =
                {
                    [relationship] = relatedEntities
                }
            };
            Reference = reference;

            Assert.Equal(reference.EntityState, Context.Reference.Entity.EntityState);
            Assert.Equal(reference.RowVersion, Context.Reference.Entity.RowVersion);
            Assert.Equal("Hello World", Context.Reference.Entity.FormattedValues["xts_attribute"]);
            Assert.Equal("my-key-1234", Context.Reference.Entity.KeyAttributes["xts_keyattribute"]);
            Assert.Same(reference.ExtensionData, Context.Reference.Entity.ExtensionData);
            Assert.Same(relatedEntities, Context.Reference.Entity.RelatedEntities[relationship]);
            Assert.Equal("Hello World", Context.Reference.GetFormattedValue(e => e.xts_attribute));
        }

        [Fact]
        public void Can_execute_operation_with_custom_wrapper()
        {
            var reference = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            Reference = reference;

            new WrapperOperation(Context).Execute();
            Assert.Equal(3000m, Input.GetValue(e => e.xts_money));
            Assert.Equal("CustomWrapper", Input.Get(e => e.xts_withcolumnattribute));
        }

        [Fact]
        public void Can_execute_late_bound_operation()
        {
            
        }

        public class LateBoundOperation : OperationBase
        {
            public LateBoundOperation(ITransactionContext<Entity> context) : base(context)
            {
            }

            protected override void HandleExecute()
            {
                var first = Get<int?>("first");
                var second = Get<int?>("second");
                var total = first + second;
                Set("total", total);
            }
        }

        public class Operation : OperationBase<xts_entity>
        {
            public Operation(ITransactionContext<xts_entity> context) : base(context)
            {
            }

            protected override void HandleExecute()
            {
                Set(e => e.xts_money, 100m);
                Set(e => e.xts_withcolumnattribute, "Hello");
            }
        }

        public class WrapperOperation : OperationBase<xts_entity, CustomEntityWrapper>
        {
            public WrapperOperation(ITransactionContext<xts_entity> context) : base(context)
            {
            }

            protected override void HandleExecute()
            {
                Set(e => e.xts_money, 3000m);
                Set(e => e.xts_withcolumnattribute, "CustomWrapper");
            }
        }

        public class CustomEntityWrapper : EntityWrapper<xts_entity>
        {
            public CustomEntityWrapper(xts_entity entity, ITransactionContextBase context) 
                : base(entity, context)
            {
            }
        }
    }
}

using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Xunit;

namespace Niam.XRM.Framework.Tests
{
    public class InternalHelperTests
    {
        [Fact]
        public void Helper_copy_entity()
        {
            var id = Guid.NewGuid();
            var reference = new EntityReference("reference", Guid.NewGuid());
            var money = new Money(100m);

            var relationship = new Relationship();
            var relatedEntities = new EntityCollection();

            var source = new Entity("entity")
            {
                Id = id,
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
            source["xts_attr1"] = reference;
            source["xts_attr2"] = money;

            var result = InternalHelper.Copy(source);

            Assert.NotSame(source, result);
            Assert.Equal(id, result.Id);
            Assert.Equal(source.LogicalName, result.LogicalName);
            Assert.Equal(source.EntityState, result.EntityState);
            Assert.Equal(source.RowVersion, result.RowVersion);
            Assert.Equal(reference, result["xts_attr1"]);
            Assert.Equal(money, result["xts_attr2"]);
            Assert.Equal("Hello World", result.FormattedValues["xts_attribute"]);
            Assert.Equal("my-key-1234", result.KeyAttributes["xts_keyattribute"]);
            Assert.Same(source.ExtensionData, result.ExtensionData);
            Assert.Same(relatedEntities, result.RelatedEntities[relationship]);

            var columnSet = new ColumnSet(true);
            result = InternalHelper.Copy(source, columnSet);
            Assert.Equal(reference, result["xts_attr1"]);
            Assert.Equal(money, result["xts_attr2"]);

            columnSet = new ColumnSet("xts_attr2");
            result = InternalHelper.Copy(source, columnSet);
            Assert.False(result.Contains("xts_attr1"));
            Assert.Equal(money, result["xts_attr2"]);
        }

        [Fact]
        public void Helper_copy_attributes()
        {
            var id = Guid.NewGuid();
            var reference = new EntityReference("entity", Guid.NewGuid()) { Name = "name" };
            var optionSet = new OptionSetValue(1);
            var money = new Money(100m);
            var number = 123;

            var attributes = new AttributeCollection();
            attributes["attr_1"] = id;
            attributes["attr_2"] = reference;
            attributes["attr_3"] = optionSet;
            attributes["attr_4"] = money;
            attributes["attr_5"] = number;

            var result = InternalHelper.Copy(attributes);

            Assert.NotSame(attributes, result);
            Assert.Equal(id, result["attr_1"]);

            Assert.NotSame(reference, result["attr_2"]);
            Assert.Equal(reference, result["attr_2"]);
            Assert.Equal(reference.Name, ((EntityReference)result["attr_2"]).Name);

            Assert.NotSame(optionSet, result["attr_3"]);
            Assert.Equal(optionSet, result["attr_3"]);

            Assert.NotSame(money, result["attr_4"]);
            Assert.Equal(money, result["attr_4"]);

            Assert.Equal(number, result["attr_5"]);
        }

        [Fact]
        public void Helper_copy_attributes_with_column_set()
        {
            var id = Guid.NewGuid();
            var reference = new EntityReference("entity", Guid.NewGuid()) { Name = "name" };
            var optionSet = new OptionSetValue(1);
            var money = new Money(100m);
            var number = 123;

            var attributes = new AttributeCollection();
            attributes["attr_1"] = id;
            attributes["attr_2"] = reference;
            attributes["attr_3"] = optionSet;
            attributes["attr_4"] = money;
            attributes["attr_5"] = number;

            var columnSet = new ColumnSet("attr_2", "attr_4");
            var result = InternalHelper.Copy(attributes, columnSet);

            Assert.False(result.Contains("attr_1"));

            Assert.NotSame(reference, result["attr_2"]);
            Assert.Equal(reference, result["attr_2"]);
            Assert.Equal(reference.Name, ((EntityReference)result["attr_2"]).Name);

            Assert.False(result.Contains("attr_3"));

            Assert.NotSame(money, result["attr_4"]);
            Assert.Equal(money, result["attr_4"]);

            Assert.False(result.Contains("attr_5"));
        }
    }
}

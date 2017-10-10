using System.Linq;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Data;
using Xunit;

namespace Niam.XRM.Framework.Tests.Data
{
    public class ColumnSetTest
    {
        [Fact]
        public void Can_implisit_cast()
        {
            ColumnSet<xts_entity> strongTypeColumnSet = null;
            ColumnSet columnSet = strongTypeColumnSet;

            Assert.Null(columnSet);
        }
        
        [Fact]
        public void Can_pass_single_attribute_on_constructor()
        {
            var strongTypeColumnSet = new ColumnSet<xts_entity>(x => x.Id);
            ColumnSet columnSet = strongTypeColumnSet;
            Assert.Single(strongTypeColumnSet.Columns);
            Assert.Single(columnSet.Columns);
            Assert.Equal("xts_entityid", columnSet.Columns[0]);
        }

        [Fact]
        public void Can_pass_attributes_on_constructor()
        {
            var strongTypeColumnSet = new ColumnSet<xts_entity>(
                e => e.Id,
                e => e.AttributeWithCaseChar
            );
            ColumnSet columnSet = strongTypeColumnSet;
            Assert.Equal(2, strongTypeColumnSet.Columns.Count());
            Assert.Equal(2, columnSet.Columns.Count);
            Assert.Equal("xts_entityid", columnSet.Columns[0]);
            Assert.Equal("attributewithcasechar", columnSet.Columns[1]);
        }

        [Fact]
        public void Can_add_single_attribute()
        {
            var strongTypeColumnSet = new ColumnSet<xts_entity>();
            ColumnSet columnSet = strongTypeColumnSet;

            strongTypeColumnSet.AddColumn(x => x.AttributeWithCaseChar);
            Assert.Single(strongTypeColumnSet.Columns);
            Assert.Single(columnSet.Columns);
            Assert.Equal("attributewithcasechar", columnSet.Columns[0]);
        }

        [Fact]
        public void Can_add_attributes()
        {
            var strongTypeColumnSet = new ColumnSet<xts_entity>();
            ColumnSet columnSet = strongTypeColumnSet;

            strongTypeColumnSet.AddColumns(
                e => e.xts_attribute, 
                e => e.xts_withcolumnattribute
            );
            Assert.Equal(2, strongTypeColumnSet.Columns.Count());
            Assert.Equal(2, columnSet.Columns.Count);
            Assert.Equal("xts_attribute", columnSet.Columns[0]);
            Assert.Equal("xts_column", columnSet.Columns[1]);
        }

        [Fact]
        public void Can_assign_derived_type_to_base_type()
        {
            var derivedColumnSet = new ColumnSet<xts_derivedentity>();
            IColumnSet<xts_entity> baseColumnSet = derivedColumnSet;
            Assert.Equal(derivedColumnSet.XrmColumnSet, baseColumnSet.XrmColumnSet);
        }
    }
}

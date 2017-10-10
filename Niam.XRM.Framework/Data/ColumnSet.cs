using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Data;

namespace Niam.XRM.Framework.Data
{
    public class ColumnSet<T> : IColumnSet<T> 
        where T : Entity
    {
        public ColumnSet XrmColumnSet { get; private set; }
        
        public IEnumerable<string> Columns => XrmColumnSet.Columns;
        
        public ColumnSet(params Expression<Func<T, object>>[] attributes)
        {
            var columns = attributes.Select(Helper.Name).ToArray();
            XrmColumnSet = new ColumnSet(columns);
        }

        public void AddColumn(Expression<Func<T, object>> attribute) => 
            XrmColumnSet.AddColumn(Helper.Name(attribute));

        public void AddColumns(params Expression<Func<T, object>>[] attributes)
        {
            var columns = attributes.Select(Helper.Name).ToArray();
            XrmColumnSet.AddColumns(columns);
        }

        public static implicit operator ColumnSet(ColumnSet<T> columnSet) => 
            columnSet?.XrmColumnSet;

        public static implicit operator ColumnSet<T>(ColumnSet columnSet) => 
            columnSet != null ? new ColumnSet<T> { XrmColumnSet = columnSet } : null;
    }
}

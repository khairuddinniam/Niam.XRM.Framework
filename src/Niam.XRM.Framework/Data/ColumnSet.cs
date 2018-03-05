using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Data;

namespace Niam.XRM.Framework.Data
{
    /// <summary>
    /// Specifies the early-bound entity attributes for which non-null values are returned from a query.
    /// </summary>
    /// <typeparam name="TEntity">Early bound entity type.</typeparam>
    public class ColumnSet<TEntity> : IColumnSet<TEntity> 
        where TEntity : Entity
    {
        /// <summary>
        /// Get instance of the Microsoft.Xrm.Sdk.Query.ColumnSet which have the equivalent data.
        /// </summary>
        /// <returns>The instance of the Microsoft.Xrm.Sdk.Query.ColumnSet </returns>
        public ColumnSet XrmColumnSet { get; private set; }

        /// <summary>
        /// Get the collection of Strings containing the names of the attributes to be retrieved from a query.
        /// </summary>
        /// <returns>The collection of attribute names to return from a query.</returns>
        public IEnumerable<string> Columns => XrmColumnSet.Columns;

        /// <summary>
        /// Initializes a new instance of the Niam.XRM.Framework.Data.ColumnSet`1 class with an array of attribute name expressions.
        /// </summary>
        /// <param name="attributes">An array of attribute name expressions to access attribute value.</param>
        public ColumnSet(params Expression<Func<TEntity, object>>[] attributes)
        {
            var columns = attributes.Select(Helper.Name).ToArray();
            XrmColumnSet = new ColumnSet(columns);
        }

        /// <summary>
        /// Adds the specified attribute to the column set.
        /// </summary>
        /// <param name="attribute">An attribute name expression to access attribute value.</param>
        public void AddColumn(Expression<Func<TEntity, object>> attribute) => 
            XrmColumnSet.AddColumn(Helper.Name(attribute));

        /// <summary>
        /// Adds the specified attribute to the column set.
        /// </summary>
        /// <param name="attributes">An array of attribute name expressions to access attribute value.</param>
        public void AddColumns(params Expression<Func<TEntity, object>>[] attributes)
        {
            var columns = attributes.Select(Helper.Name).ToArray();
            XrmColumnSet.AddColumns(columns);
        }

        /// <summary>
        /// Implicitly convert Niam.XRM.Framework.Data.ColumnSet`1 type to Microsoft.Xrm.Sdk.Query.ColumnSet type.
        /// </summary>
        /// <param name="columnSet">The object to be converted.</param>
        /// <returns>The converted type.</returns>
        public static implicit operator ColumnSet(ColumnSet<TEntity> columnSet) => 
            columnSet?.XrmColumnSet;

        /// <summary>
        /// Implicitly convert Microsoft.Xrm.Sdk.Query.ColumnSet type to Niam.XRM.Framework.Data.ColumnSet`1 type.
        /// </summary>
        /// <param name="columnSet">The object to be converted.</param>
        /// <returns>The converted type.</returns>
        public static implicit operator ColumnSet<TEntity>(ColumnSet columnSet) => 
            columnSet != null ? new ColumnSet<TEntity> { XrmColumnSet = columnSet } : null;
    }
}

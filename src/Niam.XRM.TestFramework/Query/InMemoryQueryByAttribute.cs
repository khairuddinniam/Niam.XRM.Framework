using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;

namespace Niam.XRM.TestFramework.Query
{
    public class InMemoryQueryByAttribute
    {
        private readonly IDictionary<string, Entity> _db;
        private readonly IDictionary<string, Entity[]> _pagingResults = new Dictionary<string, Entity[]>();

        public IQueryParameter QueryParameter { get; }

        public InMemoryQueryByAttribute(IDictionary<string, Entity> db, IQueryParameter queryParameter)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            QueryParameter = queryParameter ?? throw new ArgumentNullException(nameof(queryParameter));
        }

        public EntityCollection RetrieveMultiple(QueryByAttribute query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (query.EntityName == null) throw new ArgumentNullException(nameof(query.EntityName));

            if (query.PageInfo?.PagingCookie != null)
                return GetPagingResult(query.PageInfo, query.PageInfo.PagingCookie);

            IEnumerable<Entity> temp = _db.Select(p => p.Value)
                .Where(e => e.LogicalName == query.EntityName).ToArray();
            Func<Expression<Func<Entity, bool>>, Expression<Func<Entity, bool>>, Expression<Func<Entity, bool>>> combine = ExpressionHelper.AndAlso;

            var conditionExpressions = new List<Expression<Func<Entity, bool>>>();
            for (var i = 0; i < query.Attributes.Count; i++)
            {
                var condition = new ConditionExpression(query.Attributes[i], ConditionOperator.Equal, query.Values[i]);
                conditionExpressions.Add(e => ConditionExp.Equal(e, condition));
            }

            var predicate = conditionExpressions
                .Aggregate((left, right) => combine(left, right))
                .Compile();

            temp = temp.Where(predicate);

            var result = temp.Select(e => e.Copy(query.ColumnSet));
            if (query.TopCount.HasValue)
            {
                result = result.Take(query.TopCount.Value);
            }
            else if (query.PageInfo != null && query.PageInfo?.Count != 0)
            {
                var pagingCookie = Guid.NewGuid().ToString();
                _pagingResults[pagingCookie] = result.ToArray();
                return GetPagingResult(query.PageInfo, pagingCookie);
            }

            return new EntityCollection(result.ToList());
        }

        private EntityCollection GetPagingResult(PagingInfo pageInfo, string pagingCookie)
        {
            var take = pageInfo.Count;
            var skip = (pageInfo.PageNumber - 1) * pageInfo.Count;
            var totalPagingResults = _pagingResults[pagingCookie];
            var pagingResults = totalPagingResults.Skip(skip).Take(take).ToList();
            var moreRecords = skip + pagingResults.Count != totalPagingResults.Length;
            return new EntityCollection(pagingResults)
            {
                MoreRecords = moreRecords,
                PagingCookie = pagingCookie
            };
        }
    }
}

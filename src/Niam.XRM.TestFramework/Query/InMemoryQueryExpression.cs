using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;

namespace Niam.XRM.TestFramework.Query
{
    public class InMemoryQueryExpression
    {
        private readonly IDictionary<string, Entity> _db;
        private readonly IDictionary<string, Entity[]> _pagingResults = new Dictionary<string, Entity[]>();

        public IQueryParameter QueryParameter { get; }

        public InMemoryQueryExpression(IDictionary<string, Entity> db, IQueryParameter queryParameter)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            QueryParameter = queryParameter ?? throw new ArgumentNullException(nameof(queryParameter));
        }

        public EntityCollection RetrieveMultiple(QueryExpression query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (query.EntityName == null) throw new ArgumentNullException(nameof(query.EntityName));

            if (query.PageInfo?.PagingCookie != null)
                return GetPagingResult(query.PageInfo, query.PageInfo.PagingCookie);

            IEnumerable<Entity> temp = _db.Select(p => p.Value)
                .Where(e => e.LogicalName == query.EntityName).ToArray();
            Func<Expression<Func<Entity, bool>>, Expression<Func<Entity, bool>>, Expression<Func<Entity, bool>>> combine = ExpressionHelper.AndAlso;
            if (query.Criteria.FilterOperator == LogicalOperator.Or)
                combine = ExpressionHelper.OrElse;

            var predicate = query.Criteria.Conditions.Any()
                ? query.Criteria.Conditions
                    .Select(ConvertToExpression)
                    .Aggregate((left, right) => combine(left, right))
                    .Compile()
                : e => true;
            
            temp = temp.Where(predicate);
            temp = Order(temp, query);

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

        private static IEnumerable<Entity> Order(IEnumerable<Entity> entities, QueryExpression query)
        {
            if (!query.Orders.Any()) return entities;

            var firstOrder = query.Orders.First();
            var results = firstOrder.OrderType == OrderType.Ascending
                ? entities.OrderBy(e => e.GetAttributeValue<object>(firstOrder.AttributeName))
                : entities.OrderByDescending(e => e.GetAttributeValue<object>(firstOrder.AttributeName));

            for (var i = 1; i < query.Orders.Count; i++)
            {
                var order = query.Orders[i];
                results = order.OrderType == OrderType.Ascending
                    ? results.ThenBy(e => e.GetAttributeValue<object>(order.AttributeName))
                    : results.ThenByDescending(e => e.GetAttributeValue<object>(order.AttributeName));
            }

            return results;
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

        private Expression<Func<Entity, bool>> ConvertToExpression(ConditionExpression condition)
        {
            if (condition.Operator == ConditionOperator.Equal)
                return e => ConditionExp.Equal(e, condition);

            if (condition.Operator == ConditionOperator.NotEqual)
                return e => ConditionExp.NotEqual(e, condition);

            if (condition.Operator == ConditionOperator.In)
                return e => ConditionExp.In(e, condition);

            if (condition.Operator == ConditionOperator.NotIn)
                return e => ConditionExp.NotIn(e, condition);

            if (condition.Operator == ConditionOperator.Null)
                return e => ConditionExp.Null(e, condition);

            if (condition.Operator == ConditionOperator.NotNull)
                return e => ConditionExp.NotNull(e, condition);

            if (condition.Operator == ConditionOperator.EqualUserId)
                return e => ConditionExp.EqualUserId(e, condition, QueryParameter);

            if (condition.Operator == ConditionOperator.NotEqualUserId)
                return e => ConditionExp.NotEqualUserId(e, condition, QueryParameter);

            if (condition.Operator == ConditionOperator.EqualBusinessId)
                return e => ConditionExp.EqualBusinessId(e, condition, QueryParameter);

            if (condition.Operator == ConditionOperator.NotEqualBusinessId)
                return e => ConditionExp.NotEqualBusinessId(e, condition, QueryParameter);

            if (condition.Operator == ConditionOperator.On)
                return e => ConditionExp.On(e, condition);

            if (condition.Operator == ConditionOperator.NotOn)
                return e => ConditionExp.NotOn(e, condition);

            if (condition.Operator == ConditionOperator.OnOrBefore)
                return e => ConditionExp.OnOrBefore(e, condition);

            if (condition.Operator == ConditionOperator.OnOrAfter)
                return e => ConditionExp.OnOrAfter(e, condition);

            if (condition.Operator == ConditionOperator.GreaterThan)
                return e => ConditionExp.GreaterThan(e, condition);

            if (condition.Operator == ConditionOperator.LessThan)
                return e => ConditionExp.LessThan(e, condition);

            if (condition.Operator == ConditionOperator.GreaterEqual)
                return e => ConditionExp.GreaterEqual(e, condition);

            if (condition.Operator == ConditionOperator.LessEqual)
                return e => ConditionExp.LessEqual(e, condition);

            if (condition.Operator == ConditionOperator.Between)
                return e => ConditionExp.Between(e, condition);

            if (condition.Operator == ConditionOperator.NotBetween)
                return e => ConditionExp.NotBetween(e, condition);

            throw new NotImplementedException(condition.Operator.ToString());
        }
    }
}

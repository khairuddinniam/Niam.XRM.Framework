using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Infrastructure;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework
{
    public static partial class Helper
    {
        public static Expression<Func<TInput, object>> BoxExpression<TInput, TOutput>(Expression<Func<TInput, TOutput>> expression)
        {
            Expression converted = Expression.Convert(expression.Body, typeof(object));
            return Expression.Lambda<Func<TInput, object>>(converted, expression.Parameters);
        }

        public static T GetService<T>(this IServiceProvider serviceProvider) => 
            (T) serviceProvider.GetService(typeof (T));

        public static T GetRequest<T>(this ITransactionContextBase context)
            where T : OrganizationRequest
            => context.PluginExecutionContext.GetRequest<T>();

        public static T GetRequest<T>(this IPluginExecutionContext context)
            where T : OrganizationRequest
        {
            var request = Instance<T>.Create();
            var parameterNames = request.Parameters.Select(p => p.Key).ToArray();
            foreach (var parameterName in parameterNames)
            {
                if (context.InputParameters.TryGetValue(parameterName, out object parameterValue))
                    request[parameterName] = parameterValue;
            }

            return request;
        }
        
        public static EntityReference ToEntityReference<T>(this ITransactionContextEntity<T> txEntity)
            where T : Entity => txEntity.Entity.ToEntityReference();

        public static bool ContainsAny<T>(this ITransactionContextEntity<T> txEntity, 
            string attribute, params string[] otherAttributes)
            where T : Entity => txEntity.Entity.ContainsAny(attribute, otherAttributes);

        public static bool ContainsAny<T>(this ITransactionContextEntity<T> txEntity,
            Expression<Func<T, object>> attribute, params Expression<Func<T, object>>[] otherAttributes)
            where T : Entity => txEntity.Entity.ContainsAny(attribute, otherAttributes);

        public static T GetImage<T>(this EntityImageCollection images, string key)
            where T : Entity => GetImage(images, key).ToEntity<T>();

        public static Entity GetImage(this EntityImageCollection images, string key)
        {
            return images.TryGetValue(key, out var image) ? image : null;
        }

        public static T ToEntity<T>(this EntityReference reference)
            where T : Entity => ToEntity(reference).ToEntity<T>();

        public static Entity ToEntity(this EntityReference reference) => 
            new Entity(reference.LogicalName, reference.Id);

        public static decimal GetValueOrDefault(this Money money, decimal defaultValue = 0m) => 
            money?.Value ?? defaultValue;
        
        public static bool Equal<TV>(TV left, TV right) => EqualityComparer<TV>.Default.Equals(left, right);

        public static bool Equal(OptionSetValue value, Enum option) => Equal(value, option.ToOptionSetValue());

        public static bool EqualsAny(OptionSetValue value, Enum firstOption, params Enum[] otherOptions)
        {
            var options = new[] { firstOption }.Concat(otherOptions);
            return options.Any(opt => Equal(value, opt));
        }

        public static bool Equal(OptionSetValue value, int option) => Equal(value, new OptionSetValue(option));

        public static bool EqualsAny(OptionSetValue value, int firstOption, params int[] otherOptions)
        {
            var options = new[] { firstOption }.Concat(otherOptions);
            return options.Any(opt => Equal(value, opt));
        }

        public static IEnumerable<TResult> FullOuterJoin<TA, TB, TKey, TResult>(
            this IEnumerable<TA> a,
            IEnumerable<TB> b,
            Func<TA, TKey> selectKeyA,
            Func<TB, TKey> selectKeyB,
            Func<TA, TB, TKey, TResult> projection)
        {
            var defaultA = default(TA);
            var defaultB = default(TB);
            var cmp = EqualityComparer<TKey>.Default;
            var alookup = a.ToLookup(selectKeyA, cmp);
            var blookup = b.ToLookup(selectKeyB, cmp);

            var keys = new HashSet<TKey>(alookup.Select(p => p.Key), cmp);
            keys.UnionWith(blookup.Select(p => p.Key));

            var join = from key in keys
                       from xa in alookup[key].DefaultIfEmpty(defaultA)
                       from xb in blookup[key].DefaultIfEmpty(defaultB)
                       select projection(xa, xb, key);

            return join;
        }
        
        internal static MemberInfo GetMemberInfo(LambdaExpression lambda)
        {
            if (lambda == null)
                throw new ArgumentNullException(nameof(lambda));

            MemberExpression memberExpr = null;
            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if (memberExpr == null)
                throw new ArgumentException(nameof(lambda));

            return memberExpr.Member;
        }

        public static T GetOrAdd<T>(this IDictionary<string, object> storage, string key, T value)
            => GetOrAdd(storage, key, () => value);

        public static T GetOrAdd<T>(this IDictionary<string, object> storage, string key, Func<T> valueFactory)
        {
            if (storage.TryGetValue(key, out var existingValue))
                return (T) existingValue;

            var value = valueFactory();
            storage[key] = value;

            return value;
        }

        /// <summary>
        /// Deserialize json string to an instance of type T using DataContractJsonSerializer.
        /// </summary>
        public static string ToJson<T>(this T obj, DataContractJsonSerializerSettings settings = null)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(T), settings);
                serializer.WriteObject(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// Serialize from an instance of type T to json string using DataContractJsonSerializer.
        /// </summary>
        public static T FromJson<T>(this string json)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T) serializer.ReadObject(stream);
            }
        }
    }
}

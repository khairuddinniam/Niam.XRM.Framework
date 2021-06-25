using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using System.Linq;

namespace Niam.XRM.Framework.TestHelper.Tests.EarlyBound.Commands
{
    public class RetrieveTotalPrice : OperationBase<new_orderdetailsummary>
    {
        public RetrieveTotalPrice(ITransactionContext<new_orderdetailsummary> context) : base(context)
        {
        }

        protected override void HandleExecute()
        {
            var orders = GetOrders();

            var totalPrice = orders.Select(order =>
            {
                var odDetail = order.GetAliasedEntity<new_orderdetail>("od");
                return odDetail.GetValue(e => e.new_quantity) * odDetail.GetValue(e => e.new_priceperitem);
            }).Sum();

            Set(e => e.new_totalprice, totalPrice);
        }

        private new_order[] GetOrders()
        {
            var query = new QueryExpression(new_order.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition<new_order>(e => e.new_name, ConditionOperator.Equal, "TEST");

            var orderDetailLink = query.AddLink<new_order, new_orderdetail>(e => e.Id, e => e.new_orderid);
            orderDetailLink.Columns = new ColumnSet(true);
            orderDetailLink.EntityAlias = "od";

            var result = Service.RetrieveMultiple(query);

            return result.Entities?.Select(e => e.ToEntity<new_order>()).ToArray();
        }
    }
}

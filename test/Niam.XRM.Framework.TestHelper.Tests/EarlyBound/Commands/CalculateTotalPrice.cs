using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;

namespace Niam.XRM.Framework.TestHelper.Tests.EarlyBound.Commands
{
    public class CalculateTotalPrice : OperationBase
    {
        public CalculateTotalPrice(ITransactionContext<Entity> context) : base(context)
        {
        }

        protected override void HandleExecute()
        {
            var quantity = Get<int?>("new_quantity") ?? 0;
            var pricePerItem = Get<Money>("new_priceperitem");
            var totalPrice = pricePerItem.GetValueOrDefault() * quantity;

            var orderDetailSummaryRef = Get<EntityReference>("new_orderdetailsummaryid");
            var orderDetailSummary = new Entity("new_orderdetailsummary")
            {
                Id = orderDetailSummaryRef?.Id ?? Guid.NewGuid()
            };
            orderDetailSummary.Set("new_orderid", Get<EntityReference>("new_orderid"));
            orderDetailSummary.Set("new_orderdetailid", ToEntityReference());
            orderDetailSummary.Set("new_totalprice", new Money(totalPrice));

            if (orderDetailSummaryRef == null)
            {
                Service.Create(orderDetailSummary);
                Set("new_orderdetailsummaryid", orderDetailSummary.ToEntityReference());
            }
            else
            {
                Service.Update(orderDetailSummary);
            }
        }
    }
}
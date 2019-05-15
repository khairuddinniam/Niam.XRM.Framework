using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;

namespace Niam.XRM.Framework.TestHelper.Tests.LateBound.Commands
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

            var summaryRef = Get<EntityReference>("new_orderdetailsummaryid");
            var summaryId = summaryRef?.Id ?? Guid.NewGuid();
            var summary = new Entity("new_orderdetailsummary", summaryId)
                .Set("new_orderid", Get<EntityReference>("new_orderid"))
                .Set("new_orderdetailid", ToEntityReference())
                .Set("new_totalprice", new Money(totalPrice));

            if (summaryRef == null)
            {
                Service.Create(summary);
                Set("new_orderdetailsummaryid", summary.ToEntityReference());
            }
            else
            {
                Service.Update(summary);
            }
        }
    }
}
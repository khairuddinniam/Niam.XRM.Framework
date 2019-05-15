using System;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;

namespace Niam.XRM.Framework.TestHelper.Tests.EarlyBound.Commands
{
    public class CalculateTotalPrice : OperationBase<new_orderdetail>
    {
        public CalculateTotalPrice(ITransactionContext<new_orderdetail> context) : base(context)
        {
        }

        protected override void HandleExecute()
        {
            var quantity = Get(e => e.new_quantity) ?? 0;
            var pricePerItem = Get(e => e.new_priceperitem);
            var totalPrice = pricePerItem.GetValueOrDefault() * quantity;

            var summaryRef = Get(e => e.new_orderdetailsummaryid);
            var summaryId = summaryRef?.Id ?? Guid.NewGuid();
            var summary = new new_orderdetailsummary(summaryId)
                .Set(e => e.new_orderid, Get(e => e.new_orderid))
                .Set(e => e.new_orderdetailid, ToEntityReference())
                .Set(e => e.new_totalprice, totalPrice);

            if (summaryRef == null)
            {
                Service.Create(summary);
                Set(e => e.new_orderdetailsummaryid, summary.ToEntityReference());
            }
            else
            {
                Service.Update(summary);
            }
        }
    }
}
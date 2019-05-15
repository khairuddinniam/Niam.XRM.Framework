using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;

namespace Niam.XRM.Framework.TestHelper.Tests.EarlyBound.Commands
{
    public class DeleteOrderDetailSummary : OperationBase<new_orderdetail>
    {
        public DeleteOrderDetailSummary(ITransactionContext<new_orderdetail> context) : base(context)
        {
        }

        protected override void HandleExecute()
        {
            var orderDetailSummaryRef = Get(e => e.new_orderdetailsummaryid);
            if (orderDetailSummaryRef != null)
                Service.Delete(orderDetailSummaryRef);
        }
    }
}
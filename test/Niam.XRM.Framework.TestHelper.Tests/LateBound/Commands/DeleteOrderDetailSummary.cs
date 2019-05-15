using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;

namespace Niam.XRM.Framework.TestHelper.Tests.LateBound.Commands
{
    public class DeleteOrderDetailSummary : OperationBase
    {
        public DeleteOrderDetailSummary(ITransactionContext<Entity> context) : base(context)
        {
        }

        protected override void HandleExecute()
        {
            var orderDetailSummaryRef = Get<EntityReference>("new_orderdetailsummaryid");
            if (orderDetailSummaryRef != null)
                Service.Delete(orderDetailSummaryRef);
        }
    }
}
using Insurgo.Plugins.Entities;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using System;
using System.Linq;

namespace Insurgo.Plugins.Business
{
    public class CalculatePackageSummariesOnSubscriptionChange : OperationBase<cr953_subscription>
    {
        public CalculatePackageSummariesOnSubscriptionChange(ITransactionContext<cr953_subscription> context) :
            base(context)
        {
        }

        protected override void HandleExecute()
        {
            var packageRef = Get(e => e.cr953_packageid);
            var customerRef = Get(e => e.cr953_customerid);

            var valid = packageRef != null && customerRef != null;

            if (!valid) return;

            var package = GetRelated(e => e.cr953_packageid,
                new ColumnSet<cr953_package>(e => e.cr953_monthlyprice));
            if (package == null) return;

            var packageSummary = GetPackageSummary(customerRef.Id);

            var isDelete = Context.PluginExecutionContext.MessageName == "Delete";

            var total = 0.0m;
            var initialAmount = Initial.GetValue(e => e.cr953_total);
            if (!isDelete)
            {
                var qty = GetValue(e => e.cr953_qty);
                var price = package.GetValue(e => e.cr953_monthlyprice);
                total = (qty * price);
                Set(e => e.cr953_total, total);
                total -= initialAmount;
            }
            else
            {
                total -= initialAmount;
            }

            var appliedTotal = packageSummary.GetValue(e => e.cr953_totalamount) + total;

            var name = customerRef.LogicalName == Account.EntityLogicalName
                ? Service.GetReferenceName<Account>(customerRef)
                : Service.GetReferenceName<Contact>(customerRef);

            var result = new cr953_packagesummary().Set(e => e.cr953_name, name + " Summary")
                .Set(e => e.cr953_customerid, customerRef).Set(e => e.cr953_totalamount, appliedTotal);

            if (packageSummary.Id != Guid.Empty)
            {
                result.Id = packageSummary.Id;
                Service.Update(result);
                return;
            }

            Service.Create(result);
        }

        private cr953_packagesummary GetPackageSummary(Guid customerRefId)
        {
            var query = new QueryExpression(cr953_packagesummary.EntityLogicalName)
            {
                ColumnSet = new ColumnSet<cr953_packagesummary>(e => e.cr953_totalamount),
                TopCount = 1
            };
            query.Criteria.AddCondition<cr953_packagesummary>(e => e.cr953_customerid, ConditionOperator.Equal,
                customerRefId);
            var result = Service.RetrieveMultiple(query);

            return result.Entities.Any()
                ? result.Entities[0].ToEntity<cr953_packagesummary>()
                : new cr953_packagesummary();
        }
    }
}

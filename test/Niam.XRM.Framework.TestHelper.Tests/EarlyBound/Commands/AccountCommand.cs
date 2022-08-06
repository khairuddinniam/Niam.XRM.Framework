using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using System.Linq;

namespace Niam.XRM.Framework.TestHelper.Tests.EarlyBound.Commands
{
    public class AccountCommand : OperationBase<Account>
    {
        public AccountCommand(ITransactionContext<Account> context) : base(context)
        {
        }

        protected override void HandleExecute()
        {
            var accounts = GetAccounts();
            foreach (var account in accounts)
            {
                Service.Delete(Account.EntityLogicalName, account.Id);
            }

            var newAccount = new Account()
                .Set(e => e.Name, "ACCOUNT-001");
            newAccount.Id = Service.Create(newAccount);

            var update = new Account {Id = newAccount.Id}
                .Set(e => e.Name, "UPDATE-ACCOUNT");

            Service.Update(update);
        }

        private Account[] GetAccounts()
        {
            var query = new QueryExpression(Account.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false)
            };
            query.Criteria.AddCondition<Account>(e => e.StateCode, ConditionOperator.Equal,
                (int)Account.OptionSets.StateCode.Active);
            var result = Service.RetrieveMultiple(query);

            return result.Entities?.Select(e => e.ToEntity<Account>()).ToArray();
        }
    }
}

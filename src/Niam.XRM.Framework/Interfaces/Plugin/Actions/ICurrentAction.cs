namespace Niam.XRM.Framework.Interfaces.Plugin.Actions
{
    public interface ICurrentAction
    {
        bool CanExecute(ICurrentActionContext context);
        void Execute(ICurrentActionContext context);
    }
}

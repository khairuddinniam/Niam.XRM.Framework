namespace Niam.XRM.Framework.Interfaces.Plugin.Actions
{
    public interface ITargetAction
    {
        bool CanExecute(ITargetActionContext context);
        void Execute(ITargetActionContext context);
    }
}

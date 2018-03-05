namespace Niam.XRM.Framework.Interfaces.Plugin.Actions
{
    public interface IInputAction
    {
        bool CanExecute(IInputActionContext context);
        void Execute(IInputActionContext context);
    }
}

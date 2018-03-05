namespace Niam.XRM.Framework.Interfaces.Plugin.Actions
{
    public interface IReferenceAction
    {
        bool CanExecute(IReferenceActionContext context);
        void Execute(IReferenceActionContext context);
    }
}

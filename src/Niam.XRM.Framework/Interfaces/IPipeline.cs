using System;

namespace Niam.XRM.Framework.Interfaces
{
    public interface IPipeline<in TRequest, TResponse>
    {
        TResponse Handle(TRequest request, Func<TResponse> next);
    }
    
    public interface IPipeline<in TRequest>
    {
        void Handle(TRequest request, Action next);
    }
}
using System;
using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework;

public class HandlerPipeline<TRequest, TResponse> : IPipeline<TRequest, TResponse>
{
    private readonly Func<TRequest, Func<TRequest, TResponse>, TResponse> _handler;

    public HandlerPipeline(Func<TRequest, Func<TRequest, TResponse>, TResponse> handler)
    {
        _handler = handler;
    }
    
    public TResponse Handle(TRequest request, Func<TRequest, TResponse> next)
    {
        return _handler(request, next);
    }
}
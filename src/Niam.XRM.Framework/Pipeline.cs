using System;
using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework
{
    public static class Pipeline<TRequest, TResponse>
    {
        private class FuncPipeline : IPipeline<TRequest, TResponse>
        {
            private readonly Func<TRequest, Func<TResponse>, TResponse> _handler;

            public FuncPipeline(Func<TRequest, Func<TResponse>, TResponse> handler)
            {
                _handler = handler;
            }
            
            public TResponse Handle(TRequest request, Func<TResponse> next)
            {
                return _handler(request, next);
            }
        }

        public static IPipeline<TRequest, TResponse> Create(Func<TRequest, Func<TResponse>, TResponse> handler) =>
            new FuncPipeline(handler);
    }
    
    public static class Pipeline<TRequest>
    {
        private class ActionPipeline : IPipeline<TRequest>
        {
            private readonly Action<TRequest, Action> _handler;

            public ActionPipeline(Action<TRequest, Action> handler)
            {
                _handler = handler;
            }
            
            public void Handle(TRequest request, Action next)
            {
                _handler(request, next);
            }
        }

        public static IPipeline<TRequest> Create(Action<TRequest, Action> handler) =>
            new ActionPipeline(handler);
    }
}
using System;

namespace Niam.XRM.Framework.Interfaces;

public interface IPipeline {}

public interface IPipeline<TRequest, TResponse> : IPipeline
{
    TResponse Handle(TRequest request, Func<TRequest, TResponse> next);
}
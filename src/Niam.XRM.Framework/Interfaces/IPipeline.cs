using System;

namespace Niam.XRM.Framework.Interfaces;

public interface IPipeline {}

public interface IPipeline<in TRequest, TResponse> : IPipeline
{
    TResponse Handle(TRequest request, Func<TResponse> next);
}
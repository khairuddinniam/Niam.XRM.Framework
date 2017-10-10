using System;
using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework.Plugin.ValueProviders.TodayDate
{
    public interface ITodayDateValueProvider : IValueProvider<DateTime>
    {
        Guid UserId { get; set; }
    }
}

using System;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin.ValueProviders.TodayDate
{
    // TodayDateValueProvider: Get today date inside plugin based on user timezone
    // -----------------------
    // User: UTC+7
    // DateTime (user local): 25-01-2017 16.45
    // Screen input (user local): 25-01-2017 00.00
    // Screen input (utc): 24-01-2017 17.00
    // TodayDateValueProvider must produce same value as Screen input (utc): 24-01-2017 17.00
    // -----------------------
    // Sample scenario:
    // 1. Get DateTime.UtcNow => 25-01-2017 09.45
    // 2. Get timezone code from UserSettings.TimeZoneCode => 4567
    // 3. Convert to user local time using LocalTimeFromUtcTimeRequest => 25-01-2017 16.45
    // 4. Get time different: (user local time) - (utc now) = 25-01-2017 16.45 - 25-01-2017 09.45 => 07.00 (7 hours)
    // 5. Get user local date from user local time => 25-01-2017 00.00
    // 6. Convert user local date to utc date: (user local date) - (time difference) => 24-01-2017 17.00
    public class TodayDateValueProvider : ValueProviderBase<DateTime>, ITodayDateValueProvider
    {
        private readonly ITransactionContextBase _context;

        public Guid UserId { get; set; }

        public TodayDateValueProvider(ITransactionContextBase context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            UserId = context.PluginExecutionContext.InitiatingUserId;
        }

        public override DateTime GetValue() => GetTodayDate(DateTime.UtcNow);


        public DateTime GetTodayDate(DateTime todayUtcTime)
        {
            var userTimeZoneCode = GetUserTimeZoneCode();
            var userLocalTime = GetUserLocalTime(todayUtcTime, userTimeZoneCode);
            var timeDifference = userLocalTime - todayUtcTime;
            var userLocalDate = userLocalTime.Date;
            var userTodayDate = userLocalDate - timeDifference;
            return DateTime.SpecifyKind(userTodayDate, DateTimeKind.Utc);
        }

        private int GetUserTimeZoneCode()
        {
            var query = new QueryExpression("usersettings")
            {
                TopCount = 1,
                ColumnSet = new ColumnSet("timezonecode")
            };
            query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, UserId);
            var userSettings = _context.Service.RetrieveMultiple(query).Entities.First();
            return userSettings.Get<int>("timezonecode");
        }

        private DateTime GetUserLocalTime(DateTime utcTime, int timeZoneCode)
        {
            var request = new LocalTimeFromUtcTimeRequest
            {
                UtcTime = utcTime,
                TimeZoneCode = timeZoneCode
            };

            var response = (LocalTimeFromUtcTimeResponse) _context.Service.Execute(request);
            return response.LocalTime;
        }

        public static TimeSpan GetDifference(DateTime leftTime, DateTime rightTime)
            => leftTime - rightTime;

        public static DateTime GetDifference(DateTime dateTime, TimeSpan timeSpan)
            => dateTime - timeSpan;
    }
}

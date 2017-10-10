using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin.ValueProviders.TodayDate;
using Niam.XRM.TestFramework;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin.ValueProviders.TodayDate
{
    public class TodayDateValueProviderTest
    {
        public static IEnumerable<object[]> GetTimeDifferenceData()
        {
            var kinds = Enum.GetValues(typeof(DateTimeKind))
                .Cast<DateTimeKind>()
                .ToArray();

            foreach (var leftTimeKind in kinds)
            {
                foreach (var rightTimeKind in kinds)
                {
                    yield return new object[] { leftTimeKind, rightTimeKind };
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetTimeDifferenceData))]
        public void Produce_same_output_when_get_time_difference_timespan(DateTimeKind leftTimeKind, 
            DateTimeKind rightTimeKind)
        {
            var leftTime = new DateTime(2017, 1, 25, 16, 45, 0, leftTimeKind);
            var rightTime = new DateTime(2017, 1, 25, 9, 45, 0, rightTimeKind);
            var timeDifference = TodayDateValueProvider.GetDifference(leftTime, rightTime);
            Assert.Equal(TimeSpan.FromHours(7), timeDifference);
        }

        [Theory]
        [InlineData(DateTimeKind.Unspecified)]
        [InlineData(DateTimeKind.Local)]
        [InlineData(DateTimeKind.Utc)]
        public void Produce_same_output_when_get_time_difference_datetime(DateTimeKind kind)
        {
            var dateTime = new DateTime(2017, 1, 25, 0, 0, 0, kind);
            var timeSpan = TimeSpan.FromHours(7);
            var result = TodayDateValueProvider.GetDifference(dateTime, timeSpan);
            var expected = new DateTime(2017, 1, 24, 17, 0, 0, kind);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Can_get_user_today_date()
        {
            var test = new TestHelper();
            var db = test.Db;
            var context = test.CreateTransactionContext<Entity, ITransactionContext<Entity>>();

            var userId = Guid.NewGuid();
            var initiatingUserId = Guid.NewGuid();
            context.PluginExecutionContext.InitiatingUserId.Returns(initiatingUserId);

            var userSettings = new UserSettings
            {
                Id = Guid.NewGuid()
            };
            userSettings.Set(e => e.SystemUserId, userId);
            userSettings.Set(e => e.TimeZoneCode, 1234);
            db["USER-SETTINGS-001"] = userSettings;

            var utcTime = new DateTime(2017, 1, 25, 9, 45, 0, DateTimeKind.Utc);
            var localTime = new DateTime(2017, 1, 25, 16, 45, 0, DateTimeKind.Unspecified);
            context.Service.Execute(Arg.Any<LocalTimeFromUtcTimeRequest>())
                .Returns(ci =>
                {
                    var request = ci.ArgAt<LocalTimeFromUtcTimeRequest>(0);
                    Assert.Equal(1234, request.TimeZoneCode);
                    Assert.Equal(utcTime, request.UtcTime);
                    var response = new LocalTimeFromUtcTimeResponse
                    {
                        ["LocalTime"] = localTime
                    };
                    return response;
                });

            var expectedTodayDate = new DateTime(2017, 1, 24, 17, 0, 0, DateTimeKind.Utc);
            var todayDateValueProvider = new TodayDateValueProvider(context);
            Assert.Equal(initiatingUserId, todayDateValueProvider.UserId);
            todayDateValueProvider.UserId = userId;
            var todayDate = todayDateValueProvider.GetTodayDate(utcTime);
            Assert.Equal(expectedTodayDate, todayDate);
        }
    }
}

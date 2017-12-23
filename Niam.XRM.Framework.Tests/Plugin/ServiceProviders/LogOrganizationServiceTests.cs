using System;
using System.Collections.Generic;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Plugin.ServiceProviders;
using NSubstitute;
using NSubstitute.Core;
using Xunit;
using Xunit.Abstractions;

namespace Niam.XRM.Framework.Tests.Plugin.ServiceProviders
{
    public class LogOrganizationServiceTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly List<string> _logParts = new List<string>();
        private readonly IOrganizationService _service;
        private readonly IOrganizationService _logService;

        public LogOrganizationServiceTests(ITestOutputHelper output)
        {
            _output = output;
            var tracingService = Substitute.For<ITracingService>();
            tracingService.When(s => s.Trace(Arg.Any<string>(), Arg.Any<object[]>()))
                .Do(ci =>
                {
                    var format = ci.ArgAt<string>(0);
                    var args = ci.ArgAt<object[]>(1);
                    var log = args.Length > 0 ? String.Format(format, args) : format;
                    _logParts.Add(log);
                });
            _service = Substitute.For<IOrganizationService>();
            _logService = new LogOrganizationService(_service, tracingService);
        }

        public void Dispose()
        {
            var printLog = String.Join("\r\n", _logParts);
            _output.WriteLine(printLog);
        }

        [Fact]
        public void Can_log_create()
        {
            var createdId = Guid.NewGuid();
            _service.Create(Arg.Any<Entity>()).Returns(createdId);

            var entity = new Entity("account")
            {
                ["new_string"] = "hello world",
                ["new_number"] = 1234,
                ["new_optionset"] = new OptionSetValue(1),
                ["new_lookupid"] = new EntityReference("contact", Guid.NewGuid())
            };

            _logService.Create(entity);

            Assert.Equal("OrganizationService.Create", _logParts[0]);
            Assert.Equal("Request:", _logParts[1]);
            Assert.NotNull(_logParts[2]);
            Assert.Equal("Response:", _logParts[3]);
            Assert.Equal(createdId.ToString("D"), _logParts[4]);
            Assert.StartsWith("OrganizationService.Create [", _logParts[5]);
        }

        [Fact]
        public void Can_log_update()
        {
            var entity = new Entity("account")
            {
                Id = Guid.NewGuid(),
                ["new_string"] = "hello world",
                ["new_number"] = 1234,
                ["new_optionset"] = new OptionSetValue(1),
                ["new_lookupid"] = new EntityReference("contact", Guid.NewGuid())
            };

            _logService.Update(entity);

            Assert.Equal("OrganizationService.Update", _logParts[0]);
            Assert.Equal("Request:", _logParts[1]);
            Assert.NotNull(_logParts[2]);
            Assert.Equal("Response: -", _logParts[3]);
            Assert.StartsWith("OrganizationService.Update [", _logParts[4]);
        }

        [Fact]
        public void Can_log_retrieve()
        {
            var id = Guid.NewGuid();
            var entity = new Entity("account")
            {
                Id = id,
                ["new_string"] = "hello world",
                ["new_number"] = 1234,
                ["new_optionset"] = new OptionSetValue(1),
                ["new_lookupid"] = new EntityReference("contact", Guid.NewGuid())
            };
            _service.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>()).Returns(entity);

            var columnSet = new ColumnSet("new_string", "new_number", "new_optionset", "new_lookupid");
            _logService.Retrieve("account", id, columnSet);

            Assert.Equal("OrganizationService.Retrieve", _logParts[0]);
            Assert.Equal("Request:", _logParts[1]);
            Assert.NotNull(_logParts[2]);
            Assert.Equal("Response:", _logParts[3]);
            Assert.NotNull(_logParts[4]);
            Assert.StartsWith("OrganizationService.Retrieve [", _logParts[5]);
        }

        [Fact]
        public void Can_log_delete()
        {
            _logService.Delete("account", Guid.NewGuid());

            Assert.Equal("OrganizationService.Delete", _logParts[0]);
            Assert.Equal("Request:", _logParts[1]);
            Assert.NotNull(_logParts[2]);
            Assert.Equal("Response: -", _logParts[3]);
            Assert.StartsWith("OrganizationService.Delete [", _logParts[4]);
        }

        [Fact]
        public void Can_log_execute()
        {
            var whoAmIResponse = new WhoAmIResponse
            {
                ["UserId"] = Guid.NewGuid(),
                ["BusinessUnitId"] = Guid.NewGuid(),
                ["OrganizationId"] = Guid.NewGuid()
            };
            _service.Execute(Arg.Any<OrganizationRequest>()).Returns(whoAmIResponse);

            var whoAmIRequest = new WhoAmIRequest();
            _logService.Execute(whoAmIRequest);

            Assert.Equal("OrganizationService.Execute", _logParts[0]);
            Assert.Equal("Request:", _logParts[1]);
            Assert.NotNull(_logParts[2]);
            Assert.Equal("Response:", _logParts[3]);
            Assert.NotNull(_logParts[4]);
            Assert.StartsWith("OrganizationService.Execute [", _logParts[5]);
        }

        [Theory]
        [MemberData(nameof(GetLogRetrieveMultipleTestData))]
        public void Can_log_retrieve_multiple(QueryBase query)
        {
            var collection = new EntityCollection
            {
                Entities =
                {
                    new Entity("account")
                    {
                        Id = Guid.NewGuid(),
                        ["new_string"] = "hello world",
                        ["new_number"] = 1234,
                        ["new_optionset"] = new OptionSetValue(1),
                        ["new_lookupid"] = new EntityReference("contact", Guid.NewGuid())
                    }
                }
            };
            _service.RetrieveMultiple(Arg.Any<QueryBase>()).Returns(collection);

            _logService.RetrieveMultiple(query);
            Assert.Equal("OrganizationService.RetrieveMultiple", _logParts[0]);
            Assert.Equal("Request:", _logParts[1]);
            Assert.NotNull(_logParts[2]);
            Assert.Equal("Response:", _logParts[3]);
            Assert.NotNull(_logParts[4]);
            Assert.StartsWith("OrganizationService.RetrieveMultiple [", _logParts[5]);
        }

        public static IEnumerable<object[]> GetLogRetrieveMultipleTestData()
        {
            var fetchXml = new[]
            {
                "<fetch mapping='logical'>",
                    "<entity name='account'>",
                        "<attribute name='accountid'/>",
                        "<attribute name='name'/>",
                    "</entity>",
                "</fetch>"
            }.Join("");

            yield return new object[] { new FetchExpression(fetchXml) };

            var queryByAttribute = new QueryByAttribute
            {
                EntityName = "account",
                ColumnSet = new ColumnSet("new_string", "new_number", "new_optionset", "new_lookupid"),
                Attributes = { "new_string", "new_number", "new_optionset", "new_lookupid" },
                Values =
                {
                    "hello world",
                    1234,
                    new OptionSetValue(1),
                    new EntityReference("contact", Guid.NewGuid())
                },
                PageInfo = new PagingInfo
                {
                    Count = 2,
                    PageNumber = 3,
                    ReturnTotalRecordCount = true
                },
                Orders =
                {
                    new OrderExpression("new_string", OrderType.Ascending)
                }
            };

            yield return new object[] { queryByAttribute };

            var queryExpression = new QueryExpression
            {
                EntityName = "account",
                ColumnSet = new ColumnSet("new_string", "new_number", "new_optionset", "new_lookupid"),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("new_number", ConditionOperator.GreaterEqual, 12)
                    }
                },
                PageInfo = new PagingInfo
                {
                    Count = 2,
                    PageNumber = 3,
                    ReturnTotalRecordCount = true
                },
                Orders =
                {
                    new OrderExpression("new_string", OrderType.Ascending)
                }
            };

            yield return new object[] { queryExpression };
        }

        [Fact]
        public void Can_log_associate()
        {
            _logService.Associate("account", Guid.NewGuid(), new Relationship(), new EntityReferenceCollection());

            Assert.Equal("OrganizationService.Associate", _logParts[0]);
            Assert.Equal("Request:", _logParts[1]);
            Assert.NotNull(_logParts[2]);
            Assert.Equal("Response: -", _logParts[3]);
            Assert.StartsWith("OrganizationService.Associate [", _logParts[4]);
        }

        [Fact]
        public void Can_log_disassociate()
        {
            _logService.Disassociate("account", Guid.NewGuid(), new Relationship(), new EntityReferenceCollection());

            Assert.Equal("OrganizationService.Disassociate", _logParts[0]);
            Assert.Equal("Request:", _logParts[1]);
            Assert.NotNull(_logParts[2]);
            Assert.Equal("Response: -", _logParts[3]);
            Assert.StartsWith("OrganizationService.Disassociate [", _logParts[4]);
        }
    }
}

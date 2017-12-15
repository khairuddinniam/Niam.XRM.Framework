using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Niam.XRM.Framework.Tests.Plugin.ServiceProviders
{
    public class LogOrganizationServiceTests
    {
        private readonly ITestOutputHelper _output;

        public LogOrganizationServiceTests(ITestOutputHelper output)
        {
            _output = output;
        }
    }
}

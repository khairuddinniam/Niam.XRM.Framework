using System;

namespace Niam.XRM.Framework.TestHelper
{
    public class TestException : Exception
    {
        public TestException()
        {
        }

        public TestException(string message): base(message)
        {
        }
    }
}

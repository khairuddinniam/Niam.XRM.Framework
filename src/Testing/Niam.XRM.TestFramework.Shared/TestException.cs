using System;

namespace Niam.XRM.TestFramework
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

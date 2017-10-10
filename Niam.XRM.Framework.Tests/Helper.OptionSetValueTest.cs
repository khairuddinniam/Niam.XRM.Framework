using Microsoft.Xrm.Sdk;
using Xunit;

namespace Niam.XRM.Framework.Tests
{
    public class OptionSetValueHelperTest
    {
        enum TestEnum
        {
            One = 1,
            Two = 2,
            Three = 3
        }

        [Fact]
        public void OptionSetValue_to_option_set_value()
        {
            Assert.Equal(1, TestEnum.One.ToOptionSetValue().Value);
            Assert.Equal(2, TestEnum.Two.ToOptionSetValue().Value);
            TestEnum? nullEnum = null;
            Assert.Null(nullEnum.ToOptionSetValue());
        }

        [Fact]
        public void OptionSetValue_equals()
        {
            OptionSetValue nullOptionSet = null;
            Assert.False(nullOptionSet.Equal(TestEnum.One));
            Assert.True(new OptionSetValue(1).Equal(TestEnum.One));
            Assert.True(new OptionSetValue(2).Equal(TestEnum.Two));
            TestEnum? nullEnum = null;
            Assert.True(nullOptionSet.Equal(nullEnum));
            Assert.False(new OptionSetValue(1).Equal(nullEnum));
            Assert.False(new OptionSetValue(2).Equal(nullEnum));
        }

        [Fact]
        public void OptionSetValue_equals_any()
        {
            var optionSet = new OptionSetValue(2);
            Assert.False(optionSet.EqualsAny(TestEnum.One));
            Assert.True(optionSet.EqualsAny(TestEnum.Two));
            Assert.True(optionSet.EqualsAny(TestEnum.One, TestEnum.Two));

            optionSet = new OptionSetValue(3);
            Assert.False(optionSet.EqualsAny(TestEnum.One, TestEnum.Two));

            OptionSetValue nullOption = null;
            TestEnum? nullEnum = null;

            Assert.True(nullOption.EqualsAny(TestEnum.One, nullEnum, TestEnum.Two));
        }
    }
}

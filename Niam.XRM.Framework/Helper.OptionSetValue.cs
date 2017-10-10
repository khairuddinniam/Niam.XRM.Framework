using System;
using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework
{
    // Use OptionSetValueHelper class name instead partial class Helper
    // because extension method name conflict.
    public static class OptionSetValueHelper
    {
        public static OptionSetValue ToOptionSetValue(this Enum option) =>
            option != null ? new OptionSetValue(Convert.ToInt32(option)) : null;

        public static bool Equal(this OptionSetValue value, Enum option) => Helper.Equal(value, option);

        public static bool EqualsAny(this OptionSetValue value, Enum firstOption, params Enum[] otherOptions)
            => Helper.EqualsAny(value, firstOption, otherOptions);
    }
}

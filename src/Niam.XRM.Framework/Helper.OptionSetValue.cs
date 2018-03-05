using System;
using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework
{
    // Use OptionSetValueHelper class name instead partial class Helper
    // because extension method name conflict.

    /// <summary>
    /// Helper class for working with option set value.
    /// </summary>
    public static class OptionSetValueHelper
    {
        /// <summary>
        /// Convert enum value to option set value.
        /// </summary>
        /// <param name="option">Enum value to be converted.</param>
        /// <returns>Converted option set value.</returns>
        public static OptionSetValue ToOptionSetValue(this Enum option) =>
            option != null ? new OptionSetValue(Convert.ToInt32(option)) : null;

        public static bool Equal(this OptionSetValue value, Enum option) => Helper.Equal(value, option);

        public static bool Equal(this OptionSetValue value, int option) => Helper.Equal(value, option);

        public static bool EqualsAny(this OptionSetValue value, Enum firstOption, params Enum[] otherOptions)
            => Helper.EqualsAny(value, firstOption, otherOptions);

        public static bool EqualsAny(this OptionSetValue value, int firstOption, params int[] otherOptions)
            => Helper.EqualsAny(value, firstOption, otherOptions);
    }
}

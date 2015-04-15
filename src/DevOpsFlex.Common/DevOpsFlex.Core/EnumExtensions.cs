namespace DevOpsFlex.Core
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Contains methods that extend the enum struct with utility and mapping methods.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the description attribute value associated with an enum value, or if not present
        /// returns the enum value ToString() value.
        /// </summary>
        /// <param name="value">The enum value we want to get the description from.</param>
        /// <returns>The value of the description attribute present in this enum value.</returns>
        public static string GetEnumDescription(this Enum value)
        {
            Contract.Requires(value != null);

            var attributes =
                (DescriptionAttribute[]) value.GetType()
                                              .GetField(value.ToString())
                                              .GetCustomAttributes(typeof (DescriptionAttribute), false);

            return attributes.Length > 0 ?
                attributes[0].Description :
                value.ToString();
        }
    }
}

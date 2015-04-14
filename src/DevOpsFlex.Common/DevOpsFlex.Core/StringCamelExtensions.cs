namespace DevOpsFlex.Core
{
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Contains string extension methods that help with camel case naming conventions.
    /// </summary>
    public static class StringCamelExtensions
    {
        /// <summary>
        /// Gets the uppercase letters on a string as lower case. Used to minimalize camel names.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <returns>The uppercase letters on a string as lower case.</returns>
        public static string GetUpperConcat(this string value)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(value));
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

            return string.Join("", value.ToCharArray().Where(char.IsUpper))
                         .ToLower();
        }
    }
}

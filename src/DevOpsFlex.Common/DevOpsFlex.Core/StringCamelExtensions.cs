namespace DevOpsFlex.Core
{
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text.RegularExpressions;

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

        /// <summary>
        /// Gets the first letter that it finds followed by the first digit that it finds.
        /// If neither a letter or a digit is found, only one char is returned.
        /// If none is found an empty string is returned.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <returns>The first letter that it finds followed by the first digit that it finds.</returns>
        public static string GetOneCharOneDigit(this string value)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(value));
            Contract.Ensures(Contract.Result<string>() != null);
            Contract.Ensures(Contract.Result<string>().Length <= 2);

            return (value.ToCharArray().FirstOrDefault(char.IsLetter).ToString() +
                    value.ToCharArray().FirstOrDefault(char.IsDigit))
                .Replace("\0", "");
        }

        /// <summary>
        /// Gets the Sql ServiceObjective part of Azure Sql Servers DTU targets
        /// from a dimension string.
        /// </summary>
        /// <param name="value">The dimension string.</param>
        /// <returns>The Sql ServiceObjective part of Azure Sql Servers.</returns>
        public static string GetSqlServiceObjective(this string value)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(value));

            var match = Regex.Match(value, "\\s[A-Z][0-9]+\\s");

            return match.Success ?
                match.Value.Trim() :
                null;
        }
    }
}

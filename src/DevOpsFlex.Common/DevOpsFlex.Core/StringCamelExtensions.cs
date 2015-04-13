namespace DevOpsFlex.Core
{
    using System.Linq;

    public static class StringCamelExtensions
    {
        public static string GetUpperConcat(this string value)
        {
            return string.Join("", value.ToCharArray().Where(char.IsUpper))
                         .ToLower();
        }
    }
}

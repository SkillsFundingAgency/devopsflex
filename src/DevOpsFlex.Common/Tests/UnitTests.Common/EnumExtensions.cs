namespace UnitTests.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides a set of helper methods or extensions that help to deal
    /// with enums in the context of a unit test.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Checks if 2 enums have the same elements with the same enum values.
        /// </summary>
        /// <typeparam name="T1">The First enum to check for.</typeparam>
        /// <typeparam name="T2">The Second enum to check for.</typeparam>
        /// <returns>A list of fully qualified name values of enum differences.</returns>
        public static IEnumerable<string> EnumDifferences<T1, T2>()
            where T1 : struct
            where T2 : struct
        {
            var enumOneNames = Enum.GetNames(typeof(T1));
            var enumTwoNames = Enum.GetNames(typeof(T2));
            var intersected = enumOneNames.Intersect(enumTwoNames).ToList();

            var oneExceptions = enumOneNames.Except(intersected);
            var twoExceptions = enumTwoNames.Except(intersected);

            return oneExceptions.Select(e => typeof(T1).FullName + "." + e)
                                .Concat(twoExceptions.Select(e => typeof(T2).FullName + "." + e));
        }
    }
}

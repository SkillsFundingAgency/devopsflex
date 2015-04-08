namespace DevOpsFlex.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Contains useful extensions to <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Enumerates a sequence and executes an <see cref="Action{T}"/> for each enumeration.
        /// Like a Select[T] but without a return type.
        /// </summary>
        /// <typeparam name="T">The Type of the enumeration.</typeparam>
        /// <param name="source">The source enumerable.</param>
        /// <param name="action">The action we want to perform.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            Contract.Requires(source != null);

            foreach (T element in source)
            {
                action(element);
            }
        }
    }
}

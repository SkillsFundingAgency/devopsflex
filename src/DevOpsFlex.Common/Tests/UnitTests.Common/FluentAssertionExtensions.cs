namespace UnitTests.Common
{
    using System;
    using FluentAssertions;
    using FluentAssertions.Specialized;

    /// <summary>
    /// Contains extensions for the <see cref="AssertionExtensions"/> class of extensions.
    /// </summary>
    public static class FluentAssertionExtensions
    {
        /// <summary>
        /// Validates if a Pre-Code Contract (Requires) violation is thrown by the <see cref="Action"/> invocation.
        /// </summary>
        /// <param name="action">The action we want to invoke.</param>
        /// <param name="because">The reason why this should happen.</param>
        /// <param name="reasonArgs">The string args to compose the reason</param>
        /// <returns>The specialized <see cref="ExceptionAssertions{TException}"/> for fluent API continuations.</returns>
        public static ExceptionAssertions<Exception> ShouldThrowPreContract(this Action action, string because = null, params object[] reasonArgs)
        {
            var specializedAssert = action.ShouldThrow<Exception>(because, reasonArgs);

            specializedAssert.Where(e => e.GetType().Name == "ContractException")
                             .Which.Message.ToLower().Should().Contain("precondition failed:");

            return specializedAssert;
        }

        /// <summary>
        /// Validates if a Post-Code Contract (Ensures) violation is thrown by the <see cref="Action"/> invocation.
        /// </summary>
        /// <param name="action">The action we want to invoke.</param>
        /// <param name="because">The reason why this should happen.</param>
        /// <param name="reasonArgs">The string args to compose the reason</param>
        /// <returns>The specialized <see cref="ExceptionAssertions{TException}"/> for fluent API continuations.</returns>
        public static ExceptionAssertions<Exception> ShouldThrowPostContract(this Action action, string because = null, params object[] reasonArgs)
        {
            var specializedAssert = action.ShouldThrow<Exception>(because, reasonArgs);

            specializedAssert.Where(e => e.GetType().Name == "ContractException")
                             .Which.Message.ToLower().Should().Contain("postcondition failed:");

            return specializedAssert;
        }
    }
}

namespace DevOpsFlex.Data.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using UnitTests.Common;

    /// <summary>
    /// Contains test that target model methods for <see cref="SqlFirewallRule"/>.
    /// </summary>
    [TestClass]
    public class SqlFirewallRuleTest
    {
        /// <summary>
        /// Tests the Exclusions functionality with an Int32 enum.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_Exclusions_WithIntEnum()
        {
            const TestIntEnum exclusions = TestIntEnum.A | TestIntEnum.B;

            var rule = new SqlFirewallRule();
            rule.SetExclusions(exclusions);

            var result = rule.GetExclusions<TestIntEnum>();

            Assert.AreEqual(result, exclusions);
        }

        /// <summary>
        /// Tests the Exclusions functionality with an UInt32 enum.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_Exclusions_WithUIntEnum()
        {
            const TestUIntEnum exclusions = TestUIntEnum.A | TestUIntEnum.B;

            var rule = new SqlFirewallRule();
            rule.SetExclusions(exclusions);

            var result = rule.GetExclusions<TestUIntEnum>();

            Assert.AreEqual(result, exclusions);
        }

        /// <summary>
        /// Tests the Exclusions functionality with an Int64 enum.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_Exclusions_WithLongEnum()
        {
            const TestLongEnum exclusions = TestLongEnum.A | TestLongEnum.B;

            var rule = new SqlFirewallRule();
            rule.SetExclusions(exclusions);

            var result = rule.GetExclusions<TestLongEnum>();

            Assert.AreEqual(result, exclusions);
        }

        /// <summary>
        /// Tests that SetExclusions validates null and fails the contract.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_SetExclusions_WithNull_FailsContract()
        {
            var rule = new SqlFirewallRule();

            FluentAssertionExtensions.ShouldThrowPreContract(() => rule.SetExclusions(null));
        }

        /// <summary>
        /// Tests that SetExclusions validates UInt64 enums and fails the contract.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_SetExclusions_WithULongEnum_FailsContract()
        {
            const TestULongEnum exclusions = TestULongEnum.A | TestULongEnum.B;

            var rule = new SqlFirewallRule();
            
            FluentAssertionExtensions.ShouldThrowPreContract(() => rule.SetExclusions(exclusions));
        }

        /// <summary>
        /// Tests that GetExclusions validates non-enum structs and fails the contract.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetExclusions_WithNonEnumStruct_FailsContract()
        {
            var rule = new SqlFirewallRule();

            FluentAssertionExtensions.ShouldThrowPreContract(() => rule.GetExclusions<DummyStruct>());
        }

        /// <summary>
        /// Tests that GetExclusions validates UInt64 enums and fails the contract.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetExclusions_WithULongEnum_FailsContract()
        {
            var rule = new SqlFirewallRule();

            FluentAssertionExtensions.ShouldThrowPreContract(() => rule.GetExclusions<TestULongEnum>());
        }

        /// <summary>
        /// Random Int32 enum.
        /// </summary>
        [Flags]
        private enum TestIntEnum : int
        {
            A = 1,
            B = 1 << 1,
            C = 1 << 2
        }

        /// <summary>
        /// Random UInt32 enum.
        /// </summary>
        [Flags]
        private enum TestUIntEnum : uint
        {
            A = 1,
            B = 1 << 1,
            C = 1 << 2
        }

        /// <summary>
        /// Random Int64 enum.
        /// </summary>
        [Flags]
        private enum TestLongEnum : long
        {
            A = 1,
            B = 1 << 1,
            C = 1 << 2
        }

        /// <summary>
        /// Random UInt64 enum.
        /// </summary>
        [Flags]
        private enum TestULongEnum : ulong
        {
            A = 1,
            B = 1 << 1,
            C = 1 << 2
        }

        /// <summary>
        /// Represents a dummy struct that's not an enum.
        /// </summary>
        private struct DummyStruct { }
    }
}

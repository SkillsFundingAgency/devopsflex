namespace DevOpsFlex.Azure.Management.Tests.Admin
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Contains tests that deal with Administration tasks around environment variables.
    /// </summary>
    [TestClass]
    public class EnvironmentVariables
    {
        /// <summary>
        /// Sets all the required environment variables required for databases.
        /// </summary>
        [TestMethod, TestCategory("Administration")]
        public void Set_Flex_Database_EnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("FlexSaUser", "flexsa");
            Environment.SetEnvironmentVariable("FlexSaPwd", "PASSWORD");
            Environment.SetEnvironmentVariable("FlexAppUser", "flexapp");
            Environment.SetEnvironmentVariable("FlexAppPwd", "PASSWORD");
        }
    }
}

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
            Environment.SetEnvironmentVariable(FlexConfiguration.EnvFlexSaUser, "flexsa");
            Environment.SetEnvironmentVariable(FlexConfiguration.EnvFlexSaPwd, "PASSWORD");
            Environment.SetEnvironmentVariable(FlexConfiguration.EnvFlexAppUser, "flexapp");
            Environment.SetEnvironmentVariable(FlexConfiguration.EnvFlexAppPwd, "PASSWORD");
        }
    }
}

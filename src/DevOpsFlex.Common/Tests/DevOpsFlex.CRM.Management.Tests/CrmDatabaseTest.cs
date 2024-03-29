﻿namespace DevOpsFlex.CRM.Management.Tests
{
    using System;
    using System.Reactive;
    using System.Reactive.Subjects;
    using Core.Events;
    using Management;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Contains tests that target the <see cref="CrmDatabase"/> SQL wrapper object.
    /// </summary>
    [TestClass]
    public class CrmDatabaseTest
    {
        public TestContext TestContext { get; set; }

        #region Integration

        /// <summary>
        /// Tests the dropping of a Database in SQL server.
        /// The database needs to exist prior to running this Integration test.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_DropDatabase()
        {
            const string databaseName = "TestIntegration_MSCRM";

            using (var eventStream = new Subject<BuildEvent>())
            using (var db = new CrmDatabase("serveraddress", "integration", "pwd"))
            {
                eventStream.Subscribe(e => TestContext.WriteLine($"[{e.Type.ToString()}] [{e.Importance.ToString()}] {e.Message}"));

                db.DropDatabaseAsync(databaseName, eventStream.AsObserver()).Wait();
            }
        }

        /// <summary>
        /// Tests if the master database exists.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_DatabaseExists()
        {
            const string databaseName = "master";

            using (var db = new CrmDatabase("serveraddress", "integration", "pwd"))
            {
                var exists = db.DatabaseExists(databaseName).Result;
                Assert.IsTrue(exists);
            }
        }

        #endregion

    }
}

namespace DevOpsFlex.CRM.Management
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Core.Events;

    /// <summary>
    /// Represents an immutable class that wraps a CrmDabase.
    /// Runs queries that perform actions directly against it.
    /// </summary>
    public class CrmDatabase : IDisposable
    {
        private readonly SqlConnection _sqlConnection;

        /// <summary>
        /// Gets the name of the SQL Server that we are connecting to.
        /// </summary>
        public string ServerName { get; }

        /// <summary>
        /// Gets the name of the SQL Database that we are connecting to.
        /// </summary>
        public string DatabaseName { get; }

        /// <summary>
        /// Gets the SQL User username to connect to the SQL server.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Gets the SQL User password to connect to the SQL server.
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Gets the Integrated-Security setting on the SQL connection string.
        /// </summary>
        public bool IntegratedSecurity { get; private set; }

        /// <summary>
        /// Initializes an instance of <see cref="CrmDatabase"/> based on Integrated Security and pointing to the
        /// master database.
        /// </summary>
        /// <param name="serverName">The name of the SQL server that we want to target.</param>
        public CrmDatabase(string serverName)
        {
            ServerName = serverName;
            DatabaseName = "master";
            IntegratedSecurity = true;

            _sqlConnection = new SqlConnection($"Data Source={ServerName};Initial Catalog={DatabaseName};Integrated Security=True");
        }

        /// <summary>
        /// Initializes an instance of <see cref="CrmDatabase"/> based on a SQL User and pointing to the
        /// master database.
        /// </summary>
        /// <param name="serverName">The name of the SQL server that we want to target.</param>
        /// <param name="username">The username of the SQL user for the connection string.</param>
        /// <param name="password">The password of the SQL user for the connection string.</param>
        public CrmDatabase(string serverName, string username, string password)
        {
            ServerName = serverName;
            DatabaseName = "master";
            IntegratedSecurity = false;
            Username = username;
            Password = password;

            _sqlConnection = new SqlConnection($"Data Source={ServerName};Initial Catalog={DatabaseName};User Id={Username};Password={Password}");
        }

        /// <summary>
        /// Drops a specific CrmDatabase, assumes that the <see cref="CrmDatabase"/> object is pointing to the master database.
        /// Throws otherwise.
        /// </summary>
        /// <remarks>
        /// This method cannot wrap anything in a transaction because of the "ALTER DATABASE" statement.
        /// </remarks>
        /// <param name="crmDatabaseName">The name of the CrmDatabase that we want to drop.</param>
        /// <param name="eventStream">The Rx event stream used to push build events onto.</param>
        public async Task DropDatabaseAsync(string crmDatabaseName, IObserver<BuildEvent> eventStream)
        {
            using (_sqlConnection)
            {
                eventStream.OnNext(new BuildEvent(BuildEventType.Information, BuildEventImportance.Medium, "Connecting to CRM SQL Server"));

                await _sqlConnection.OpenAsync();

                eventStream.OnNext(new BuildEvent(BuildEventType.Information, BuildEventImportance.Medium, $"Starting the Database drop execution for {crmDatabaseName}"));

                using (var cmd = _sqlConnection.CreateCommand())
                {
                    cmd.Connection = _sqlConnection;
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = $"EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = {crmDatabaseName}";
                    await cmd.ExecuteNonQueryAsync();

                    eventStream.OnNext(new BuildEvent(BuildEventType.Information, BuildEventImportance.Medium, $"[{crmDatabaseName}] Successfully deleted backup history"));

                    cmd.CommandText = $"ALTER DATABASE [{crmDatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    await cmd.ExecuteNonQueryAsync();

                    eventStream.OnNext(new BuildEvent(BuildEventType.Information, BuildEventImportance.Medium, $"[{crmDatabaseName}] Successfully closed all connections, except this one, on the database."));

                    cmd.CommandText = $"DROP DATABASE [{crmDatabaseName}]";
                    await cmd.ExecuteNonQueryAsync();

                    eventStream.OnNext(new BuildEvent(BuildEventType.Information, BuildEventImportance.Medium, $"[{crmDatabaseName}] Successfully droped the database"));
                }
            }
        }

        /// <summary>
        /// Checks if a given SQL Database exists or not on the server.
        /// </summary>
        /// <param name="crmDatabaseName">The name of the CrmDatabase that we want to check for existence.</param>
        /// <returns>True if the database exists on the target SQL server, false otherwise.</returns>
        public async Task<bool> DatabaseExists(string crmDatabaseName)
        {
            try
            {
                using (var cmd = _sqlConnection.CreateCommand())
                {
                    await _sqlConnection.OpenAsync();
                    cmd.CommandText = $"SELECT db_id('{crmDatabaseName}')";
                    return (await cmd.ExecuteScalarAsync() as DBNull) == null;
                }
            }
            finally
            {
                _sqlConnection.Close();
            }

        }

        /// <summary>
        /// Defines a method to release allocated resources.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Defines a method to release allocated resources.
        /// </summary>
        /// <param name="disposing">If we are already disposing or not.</param>
        [ExcludeFromCodeCoverage]
        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (_sqlConnection != null)
            {
                if (_sqlConnection.State != ConnectionState.Closed)
                {
                    _sqlConnection.Close();
                }

                _sqlConnection.Dispose();
            }
        }
    }
}

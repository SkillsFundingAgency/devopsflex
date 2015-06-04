namespace DevOpsFlex.Azure.Management
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an immutable class that wraps a CrmDabase.
    /// Runs queries that perform actions directly against it.
    /// </summary>
    public class DevOpsAzureDatabase : IDisposable
    {
        /// <summary>
        /// Keeps track of when this object has been disposed of.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Holds the connection to SQL.
        /// </summary>
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
        /// Initializes an instance of <see cref="DevOpsAzureDatabase"/> based on a SQL User and pointing to the
        /// master database.
        /// </summary>
        /// <param name="serverName">The name of the SQL server that we want to target.</param>
        /// <param name="databaseName">The name of the SQL database for the connection string.</param>
        /// <param name="username">The username of the SQL user for the connection string.</param>
        /// <param name="password">The password of the SQL user for the connection string.</param>
        public DevOpsAzureDatabase(string serverName, string databaseName, string username, string password)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(serverName));
            Contract.Requires(!serverName.Contains("database.windows.net"));
            Contract.Requires(!string.IsNullOrWhiteSpace(databaseName));
            Contract.Requires(!string.IsNullOrWhiteSpace(username));
            Contract.Requires(!string.IsNullOrWhiteSpace(password));

            ServerName = serverName + ".database.windows.net";
            DatabaseName = databaseName;
            Username = username;
            Password = password;

            _sqlConnection = new SqlConnection($"Data Source={ServerName};Initial Catalog={DatabaseName};User Id={Username};Password={Password}");
        }

        /// <summary>
        /// Creates a new database user on the current database for a specific server login.
        /// </summary>
        /// <param name="loginName">The login name that we are creating the user for.</param>
        /// <param name="userName">The database username.</param>
        /// <param name="defaultSchema">The user's default schema.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public async Task CreateDatabaseUserAsync(string loginName, string userName, string defaultSchema)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(loginName));
            Contract.Requires(!string.IsNullOrWhiteSpace(userName));
            Contract.Requires(!string.IsNullOrWhiteSpace(defaultSchema));

            await _sqlConnection.OpenAsync();

            try
            {
                using (var cmd = _sqlConnection.CreateCommand())
                {
                    cmd.Connection = _sqlConnection;
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = $"CREATE USER {userName} FOR LOGIN {loginName} WITH DEFAULT_SCHEMA = {defaultSchema}";
                    await cmd.ExecuteNonQueryAsync();

                    if (defaultSchema == "dbo")
                    {
                        cmd.CommandText = $"EXEC sp_addrolemember N'db_owner', N'{userName}'";
                        await cmd.ExecuteNonQueryAsync();
                    }
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
            if (_disposed || !disposing)
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

            _disposed = true;
        }
    }
}

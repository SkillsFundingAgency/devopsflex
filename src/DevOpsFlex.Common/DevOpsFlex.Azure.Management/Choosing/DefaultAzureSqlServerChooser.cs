namespace DevOpsFlex.Azure.Management.Choosing
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.Models;
    using Microsoft.WindowsAzure.Management.Sql;

    /// <summary>
    /// Provides a default SQL Server chooser object.
    /// </summary>
    public class DefaultAzureSqlServerChooser : IChooseSqlServer
    {
        /// <summary>
        /// Finds a valid SQL Server in the subscription to create web sites on.
        /// </summary>
        /// <param name="client">The <see cref="SqlManagementClient"/> that is performing the operation.</param>
        /// <param name="location">The Azure location where we want to create the SQL Server. This is a value in <see cref="LocationNames"/></param>
        /// <returns>A suitable sql server name if one is found, null otherwise.</returns>
        public async Task<string> Choose(SqlManagementClient client, string location)
        {
            var sqlServer = (await client.Servers.ListAsync()).FirstOrDefault(s => s.Location == location);

            return sqlServer == null ?
                null :
                sqlServer.Name;
        }
    }
}

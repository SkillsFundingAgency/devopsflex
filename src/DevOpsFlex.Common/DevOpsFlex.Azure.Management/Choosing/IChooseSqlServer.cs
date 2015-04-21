namespace DevOpsFlex.Azure.Management.Choosing
{
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.Models;
    using Microsoft.WindowsAzure.Management.Sql;

    /// <summary>
    /// Provides a contract to override the SQL Server choosing pipeline.
    /// </summary>
    public interface IChooseSqlServer
    {
        /// <summary>
        /// Finds a valid SQL Server in the subscription to create web sites on.
        /// </summary>
        /// <param name="client">The <see cref="SqlManagementClient"/> that is performing the operation.</param>
        /// <param name="location">The Azure location where we want to create the SQL Server. This is a value in <see cref="LocationNames"/></param>
        /// <returns>A suitable sql server name if one is found, null otherwise.</returns>
        Task<string> Choose(SqlManagementClient client, string location);
    }
}
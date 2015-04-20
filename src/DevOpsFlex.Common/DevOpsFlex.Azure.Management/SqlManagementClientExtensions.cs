namespace DevOpsFlex.Azure.Management
{
    using System.Threading.Tasks;
    using Hyak.Common;
    using Microsoft.WindowsAzure.Management.Sql;
    using Microsoft.WindowsAzure.Management.Sql.Models;

    public static class SqlManagementClientExtensions
    {
        public static async Task CheckCreateDatabase(
            this SqlManagementClient client,
            string serverName,
            string databaseName,
            string databaseEdition,
            string collationName,
            int sizeInGb)
        {
            DatabaseGetResponse ns = null;

            try
            {
                ns = await client.Databases.GetAsync(serverName, databaseName);
            }
            catch (CloudException cex)
            {
                if (!cex.Error.Message.Contains(string.Format("Resource with the name '{0}' does not exist", databaseName))) throw;
            }

            if (ns != null) return;

            await client.Databases.CreateAsync(
                serverName,
                new DatabaseCreateParameters
                {
                    Name = databaseName,
                    Edition = databaseEdition,
                    CollationName = collationName,
                    MaximumDatabaseSizeInGB = sizeInGb
                });
        }
    }
}

namespace DevOpsFlex.PowerShell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Wraps an operation to move table storage around. Included Table Storage schema and data.
    /// </summary>
    public class CopyAzureTablesOperation
    {
        private readonly CloudStorageAccount _sourceAccount;
        private readonly CloudStorageAccount _targetAccount;
        private readonly string _regexFilter;

        private readonly Dictionary<string, long> _retrieved = new Dictionary<string, long>();
        private readonly TableQuery<DynamicTableEntity> _query = new TableQuery<DynamicTableEntity>();

        /// <summary>
        /// Instantiates a new instance of <see cref="CopyAzureTablesOperation"/>.
        /// </summary>
        /// <param name="sourceAccount">The source <see cref="CloudStorageAccount"/> that the copy will use.</param>
        /// <param name="targetAccount">The target <see cref="CloudStorageAccount"/> that the copy will use.</param>
        /// <param name="regexFilter">If this is populated (not null and not blank) it will be applied to filter IN table names that are being copied.</param>
        public CopyAzureTablesOperation(CloudStorageAccount sourceAccount, CloudStorageAccount targetAccount, string regexFilter)
        {
            _sourceAccount = sourceAccount;
            _targetAccount = targetAccount;
            _regexFilter = regexFilter;
        }

        /// <summary>
        /// Executes the <see cref="CopyAzureTablesOperation"/>.
        /// </summary>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public async Task Execute()
        {
            IEnumerable<CloudTable> cloudTables = _sourceAccount.CreateCloudTableClient()
                                                                .ListTables()
                                                                .OrderBy(t => t.Name);

            if (!string.IsNullOrWhiteSpace(_regexFilter))
            {
                cloudTables = cloudTables.Where(t => Regex.IsMatch(t.Name, _regexFilter));
            }

            foreach (var table in cloudTables)
            {
                await CopyTables(table);
            }
        }

        private async Task CopyTables(CloudTable table)
        {
            var targetTable = _targetAccount.CreateCloudTableClient()
                                            .GetTableReference(table.Name);

            await targetTable.CreateIfNotExistsAsync();
            await targetTable.SetPermissionsAsync(table.GetPermissions());

            ExecuteQuerySegment(table, null);
        }

        private void ExecuteQuerySegment(CloudTable table, TableContinuationToken token)
        {
            table.BeginExecuteQuerySegmented(_query, token, new TableRequestOptions(), new OperationContext { ClientRequestID = "StorageMigrator" }, HandleCompletedQuery(), table);
        }

        private AsyncCallback HandleCompletedQuery()
        {
            return ar =>
            {
                var cloudTable = ar.AsyncState as CloudTable;
                if (cloudTable == null) return;

                var response = cloudTable.EndExecuteQuerySegmented<DynamicTableEntity>(ar);
                var token = response.ContinuationToken;

                if (token != null)
                {
                    Task.Run(() => ExecuteQuerySegment(cloudTable, token));
                }

                var retrieved = response.Count();

                if (retrieved > 0)
                {
                    Task.Run(() => WriteToTarget(cloudTable, response));
                }

                var recordsRetrieved = retrieved;
                UpdateCount(cloudTable, recordsRetrieved);

                Console.WriteLine("Table " + cloudTable.Name + " |> Records = " + recordsRetrieved + " | Total Records = " + _retrieved[cloudTable.Name]);
            };
        }

        private void UpdateCount(CloudTable cloudTable, int recordsRetrieved)
        {
            if (!_retrieved.ContainsKey(cloudTable.Name))
            {
                _retrieved.Add(cloudTable.Name, recordsRetrieved);
            }
            else
            {
                _retrieved[cloudTable.Name] += recordsRetrieved;
            }
        }

        private void WriteToTarget(CloudTable cloudTable, IEnumerable<DynamicTableEntity> response)
        {
            var writer = new TableStorageWriter(_targetAccount, cloudTable.Name + "target");

            foreach (var entity in response)
            {
                writer.InsertOrReplace(entity);
            }

            writer.Execute();
        }
    }
}

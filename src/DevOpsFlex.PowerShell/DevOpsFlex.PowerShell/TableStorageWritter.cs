namespace DevOpsFlex.PowerShell
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Enables the batch writing to Azure Table Storage to use in the <see cref="CopyAzureTablesOperation"/>.
    /// </summary>
    public class TableStorageWriter
    {
        private const int BatchSize = 50;
        private readonly ConcurrentQueue<Tuple<ITableEntity, TableOperation>> _operations;
        private readonly CloudStorageAccount _storageAccount;
        private readonly string _tableName;

        /// <summary>
        /// Creates a new instance of <see cref="TableStorageWriter"/>.
        /// </summary>
        /// <param name="storageAccount">The <see cref="CloudStorageAccount"/> that we are writing the data to.</param>
        /// <param name="tableName">The name of the target table that we are writing to.</param>
        public TableStorageWriter(CloudStorageAccount storageAccount, string tableName)
        {
            _tableName = tableName;
            _storageAccount = storageAccount;

            var tableReference = MakeTableReference();
            tableReference.CreateIfNotExists();

            _operations = new ConcurrentQueue<Tuple<ITableEntity, TableOperation>>();
        }

        /// <summary>
        /// Gets the number of outstanding operations in this batch.
        /// </summary>
        public decimal OutstandingOperations => _operations.Count;

        /// <summary>
        /// Performs an Enqueue in the batch for an Insert operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <see cref="ITableEntity"/> that we want to insert.</typeparam>
        /// <param name="entity">The entity that we want to insert.</param>
        public void Insert<TEntity>(TEntity entity)
            where TEntity : ITableEntity
        {
            _operations.Enqueue(new Tuple<ITableEntity, TableOperation>(entity, TableOperation.Insert(entity)));
        }

        /// <summary>
        /// Performs an Enqueue in the batch for a Delete operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <see cref="ITableEntity"/> that we want to delete.</typeparam>
        /// <param name="entity">The entity that we want to delete.</param>
        public void Delete<TEntity>(TEntity entity)
            where TEntity : ITableEntity
        {
            _operations.Enqueue(new Tuple<ITableEntity, TableOperation>(entity, TableOperation.Delete(entity)));
        }

        /// <summary>
        /// Performs an Enqueue in the batch for an InsertOrMerge operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <see cref="ITableEntity"/> that we want to insert or merge.</typeparam>
        /// <param name="entity">The entity that we want to insert or merge.</param>
        public void InsertOrMerge<TEntity>(TEntity entity)
            where TEntity : ITableEntity
        {
            _operations.Enqueue(new Tuple<ITableEntity, TableOperation>(entity, TableOperation.InsertOrMerge(entity)));
        }

        /// <summary>
        /// Performs an Enqueue in the batch for an InsertOrReplace operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <see cref="ITableEntity"/> that we want to insert or replace.</typeparam>
        /// <param name="entity">The entity that we want to insert or replace.</param>
        public void InsertOrReplace<TEntity>(TEntity entity)
            where TEntity : ITableEntity
        {
            _operations.Enqueue(new Tuple<ITableEntity, TableOperation>(entity, TableOperation.InsertOrReplace(entity)));
        }

        /// <summary>
        /// Performs an Enqueue in the batch for an Merge operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <see cref="ITableEntity"/> that we want to merge.</typeparam>
        /// <param name="entity">The entity that we want to merge.</param>
        public void Merge<TEntity>(TEntity entity)
            where TEntity : ITableEntity
        {
            _operations.Enqueue(new Tuple<ITableEntity, TableOperation>(entity, TableOperation.Merge(entity)));
        }

        /// <summary>
        /// Performs an Enqueue in the batch for an Replace operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <see cref="ITableEntity"/> that we want to replace.</typeparam>
        /// <param name="entity">The entity that we want to replace.</param>
        public void Replace<TEntity>(TEntity entity)
            where TEntity : ITableEntity
        {
            _operations.Enqueue(new Tuple<ITableEntity, TableOperation>(entity, TableOperation.Replace(entity)));
        }

        /// <summary>
        /// Executes all the batched operations in the <see cref="TableStorageWriter"/>.
        /// </summary>
        public void Execute()
        {
            var count = _operations.Count;
            var toExecute = new List<Tuple<ITableEntity, TableOperation>>();

            for (var index = 0; index < count; index++)
            {
                Tuple<ITableEntity, TableOperation> operation;
                _operations.TryDequeue(out operation);
                if (operation != null)
                    toExecute.Add(operation);
            }

            toExecute.GroupBy(tuple => tuple.Item1.PartitionKey)
                     .ToList()
                     .ForEach(g =>
                     {
                         var opreations = g.ToList();

                         var batch = 0;
                         var operationBatch = GetOperations(opreations, batch);

                         while (operationBatch.Any())
                         {
                             var tableBatchOperation = MakeBatchOperation(operationBatch);

                             ExecuteBatchWithRetries(tableBatchOperation);

                             batch++;
                             operationBatch = GetOperations(opreations, batch);
                         }
                     });
        }

        private static TableBatchOperation MakeBatchOperation(List<Tuple<ITableEntity, TableOperation>> operationsToExecute)
        {
            var tableBatchOperation = new TableBatchOperation();
            operationsToExecute.ForEach(tuple => tableBatchOperation.Add(tuple.Item2));
            return tableBatchOperation;
        }

        private static List<Tuple<ITableEntity, TableOperation>> GetOperations(IEnumerable<Tuple<ITableEntity, TableOperation>> opreations, int batch)
        {
            return opreations.Skip(batch * BatchSize)
                             .Take(BatchSize)
                             .ToList();
        }

        private CloudTable MakeTableReference()
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var tableReference = tableClient.GetTableReference(_tableName);
            return tableReference;
        }

        private void ExecuteBatchWithRetries(TableBatchOperation tableBatchOperation)
        {
            var tableRequestOptions =
                new TableRequestOptions
                {
                    RetryPolicy = new ExponentialRetry(TimeSpan.FromMilliseconds(2), 100)
                };

            var tableReference = MakeTableReference();

            tableReference.ExecuteBatch(tableBatchOperation, tableRequestOptions);
        }
    }
}

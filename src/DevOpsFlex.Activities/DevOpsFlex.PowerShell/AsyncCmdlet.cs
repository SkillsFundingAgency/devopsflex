namespace DevOpsFlex.PowerShell
{
    using System.Management.Automation;
    using System.Threading.Tasks;

    /// <summary>
    /// Serves as a base class for derived cmdlets that need to run async work and that do not
    /// depend on the Windows PowerShell runtime and that can be called directly from within another cmdlet.
    /// </summary>
    public class AsyncCmdlet : Cmdlet
    {
        /// <summary>
        /// The adapter that queues work to be done on the main thread.
        /// </summary>
        protected readonly ThreadQueue ThreadAdapter = new ThreadQueue();

        /// <summary>
        /// Process a block of asynchronous work coming in as an array of <see cref="Task"/>.
        /// </summary>
        /// <param name="tasks">The block of asyncronous work to be performed.</param>
        protected void ProcessAsyncWork(Task[] tasks)
        {
            var workers = tasks.Length;

            foreach (var task in tasks)
            {
                task.ContinueWith(_ => { if (--workers == 0) ThreadAdapter.Complete(); });
            }

            AsyncPump.Run(async () => await ThreadAdapter.Listen(o => WriteVerbose((string)o)));

            Task.WaitAll(tasks);
        }
    }
}

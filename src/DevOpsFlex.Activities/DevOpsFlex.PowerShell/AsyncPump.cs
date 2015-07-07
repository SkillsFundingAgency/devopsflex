namespace DevOpsFlex.PowerShell
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a pump that supports running asynchronous methods on the current thread.
    ///
    ///     This is taken from Stephen Taub's excelent post here:
    ///     http://blogs.msdn.com/b/pfxteam/archive/2012/01/20/10259049.aspx
    /// 
    ///     Have a read if you want some insights on the "magic" that await does returning you to the
    ///     right execution context after you call await.
    /// </summary>
    public static class AsyncPump
    {
        /// <summary>
        /// Runs the specified asynchronous function.
        /// </summary>
        /// <param name="func">The asynchronous function to execute.</param>
        public static void Run(Func<Task> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            var prevCtx = SynchronizationContext.Current;
            try
            {
                var syncCtx = new SingleThreadSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(syncCtx);

                var t = func();
                if (t == null)
                {
                    throw new InvalidOperationException("No task provided.");
                }

                t.ContinueWith(delegate { syncCtx.Complete(); }, TaskScheduler.Default);

                syncCtx.RunOnCurrentThread();
                t.GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevCtx);
            }
        }

        /// <summary>
        /// Provides a SynchronizationContext that's single-threaded.
        /// </summary>
        private sealed class SingleThreadSynchronizationContext : SynchronizationContext
        {
            /// <summary>
            /// The queue of work items.
            /// </summary>
            private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object>> _queue = 
                new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();

            /// <summary>
            /// Dispatches an asynchronous message to the synchronization context.
            /// </summary>
            /// <param name="d">The System.Threading.SendOrPostCallback delegate to call.</param>
            /// <param name="state">The object passed to the delegate.</param>
            public override void Post(SendOrPostCallback d, object state)
            {
                if (d == null)
                {
                    throw new ArgumentNullException(nameof(d));
                }

                _queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            public override void Send(SendOrPostCallback _, object __)
            {
                throw new NotSupportedException("Synchronously sending is not supported.");
            }

            /// <summary>
            /// Runs an loop to process all queued work items.
            /// </summary>
            public void RunOnCurrentThread()
            {
                foreach (var workItem in _queue.GetConsumingEnumerable())
                {
                    workItem.Key(workItem.Value);
                }
            }

            /// <summary>
            /// Notifies the context that no more work will arrive.
            /// </summary>
            public void Complete()
            {
                _queue.CompleteAdding();
            }
        }
    }
}
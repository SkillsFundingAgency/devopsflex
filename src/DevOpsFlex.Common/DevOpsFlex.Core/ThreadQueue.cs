namespace DevOpsFlex.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a way to queue work onto a specific thread.
    /// This blocks the thread that the work is being queued on and needs to be
    /// specificaly completed for the thread to continue.
    /// </summary>
    public class ThreadQueue
    {
        /// <summary>
        /// Defines the interval between listening again when the queue reaches 0
        /// but the queue isn't completed.
        /// </summary>
        private const int ListeningDelay = 100;

        private readonly Queue<object> _queue = new Queue<object>();
        private readonly object _queueGate = new object();
        private bool _completed;

        /// <summary>
        /// Starts listening for work on the thread this object was instantiated on.
        /// </summary>
        /// <remarks>
        /// This blocks the thread the object was instantiated on.
        /// </remarks>
        /// <param name="action">The <see cref="Action{T}"/> delegate we want to run for each queued object.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public async Task ListenAsync(Action<object> action)
        {
            while (!_completed || _queue.Count > 0)
            {
                while (_queue.Count > 0)
                {
                    object obj;
                    lock (_queueGate)
                    {
                        obj = _queue.Dequeue();
                    }

                    action(obj);
                }

                await Task.Delay(ListeningDelay);
            }
        }

        /// <summary>
        /// Queues a object for a unit of work onto this <see cref="System.Threading.Thread"/> queue.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> param that will be passed to the unit of work.</param>
        public void QueueObject(object obj)
        {
            lock (_queueGate)
            {
                _queue.Enqueue(obj);
            }
        }

        /// <summary>
        /// Completes this queue and imediatly stops the listener.
        /// </summary>
        public void Complete()
        {
            _completed = true;
        }
    }
}

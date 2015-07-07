using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsFlex.Azure.Management.Tests
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reactive.Subjects;
    using System.Threading;
    using System.Threading.Tasks;

    [TestClass]
    public class RxSchedulerProof
    {
        private readonly Subject<int> _subject = new Subject<int>();
        private readonly Subject<object> _routeSubject = new Subject<object>();

        [TestMethod]
        public void Foo()
        {
            Debug.WriteLine($"MAINID {Thread.CurrentThread.ManagedThreadId}");

            var adapter = new ThreadQueue();

            // ReSharper disable once ImplicitlyCapturedClosure
            _subject.Subscribe(i => adapter.QueueObject(i));

            _routeSubject.Subscribe(o => Debug.WriteLine($"THISID {Thread.CurrentThread.ManagedThreadId} TID {o}"));

            var t1 = DoSomething();
            var t2 = DoSomething();
            var t3 = DoSomething();
            var t4 = DoSomething();
            var t5 = DoSomething();
            var t6 = DoSomething();

            AsyncPump.Run(

// Async method lacks 'await' operators and will run synchronously
#pragma warning disable CS1998
                async delegate
#pragma warning restore CS1998

                {
                    Debug.WriteLine($"PUMPID {Thread.CurrentThread.ManagedThreadId}");

// Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS4014
                    adapter.Listen(o => _routeSubject.OnNext(o));
#pragma warning restore CS4014

                    Task.WaitAll(t1, t2, t3, t4, t5, t6);
                    adapter.Complete();
                });
        }

        private async Task DoSomething()
        {
            Debug.WriteLine($"TID {Thread.CurrentThread.ManagedThreadId}");

            _subject.OnNext(Thread.CurrentThread.ManagedThreadId);
            await Task.Delay(10000);
        }
    }

    public class ThreadQueue
    {
        private readonly Queue<object> _queue = new Queue<object>();
        private readonly object _queueGate = new object();
        private bool _completed;

        public async Task Listen(Action<object> action)
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

                await Task.Delay(100);
            }
        }

        public void QueueObject(object obj)
        {
            lock (_queueGate)
            {
                _queue.Enqueue(obj);
            }
        }

        public void Complete()
        {
            _completed = true;
        }
    }

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
                throw new ArgumentNullException("func");
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
            public override void Send(SendOrPostCallback d, object state)
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

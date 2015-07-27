namespace DevOpsFlex.Data
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Core;
    using Events;

    /// <summary>
    /// Central point in DevOpsFlex to hold all the event streams.
    /// </summary>
    public static class FlexStreams
    {
        private static ThreadQueue _threadQueue;

        /// <summary>
        /// Holds a static reference to the build event stream.
        /// </summary>
        private static readonly Subject<BuildEvent> BuildEventStream = new Subject<BuildEvent>();

        /// <summary>
        /// Gets the build event stream as an <see cref="IObservable{T}"/>.
        /// </summary>
        public static IObservable<BuildEvent> BuildEventsObservable => BuildEventStream.AsObservable();

        public static void Publish(BuildEvent buildEvent)
        {
            if (_threadQueue != null)
            {
                _threadQueue.QueueObject(buildEvent);
            }
            else
            {
                BuildEventStream.OnNext(buildEvent);
            }
        }

        public static void UseThreadQueue(ThreadQueue threadQueue)
        {
            _threadQueue = threadQueue;
        }
    }
}

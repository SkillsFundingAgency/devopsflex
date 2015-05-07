﻿namespace DevOpsFlex.Azure.Management
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Data.Events;

    /// <summary>
    /// Central point in DevOpsFlex to hold all the event streams.
    /// </summary>
    public static class FlexStreams
    {
        /// <summary>
        /// Holds a static reference to the build event stream.
        /// </summary>
        private static readonly Subject<BuildEvent> BuildEventStream = new Subject<BuildEvent>();

        /// <summary>
        /// Gets the build event stream as an <see cref="IObserver{T}"/>.
        /// </summary>
        public static IObserver<BuildEvent> BuildEventsObserver
        {
            get { return BuildEventStream.AsObserver(); }
        }

        /// <summary>
        /// Gets the build event stream as an <see cref="IObservable{T}"/>.
        /// </summary>
        public static IObservable<BuildEvent> BuildEventsObservable
        {
            get { return BuildEventStream.AsObservable(); }
        }

    }
}

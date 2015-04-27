namespace DevOpsFlex.Core
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Events;

    public static class FlexCore
    {
        private static readonly Subject<BuildEvent> BuildEventStream = new Subject<BuildEvent>();

        public static IObserver<BuildEvent> BuildEventsObserver
        {
            get { return BuildEventStream.AsObserver(); }
        }

        public static IObservable<BuildEvent> BuildEventsObservable
        {
            get { return BuildEventStream.AsObservable(); }
        }
    }
}

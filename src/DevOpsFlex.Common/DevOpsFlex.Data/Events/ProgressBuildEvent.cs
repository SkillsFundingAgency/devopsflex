namespace DevOpsFlex.Data.Events
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics.CodeAnalysis;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    /// <summary>
    /// Represents a progress build event. The progress ticks are pushed through an <see cref="IObservable{T}"/>
    /// to support fancy subscriptions and fancy logic around that.
    /// </summary>
    public sealed class ProgressBuildEvent : BuildEvent, IDisposable
    {
        /// <summary>
        /// Keeps track of when this object has been disposed of.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Holds the actual progress <see cref="Subject{T}"/>.
        /// </summary>
        private readonly Subject<int> _progressSubject;

        /// <summary>
        /// Gets the Progress event stream as a Rx <see cref="IObservable{T}"/>.
        /// </summary>
        [NotMapped]
        public IObservable<int> ProgressStream
        {
            get { return _progressSubject.AsObservable(); }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ProgressBuildEvent"/>.
        /// </summary>
        /// <param name="message">The text message that describes the event.</param>
        public ProgressBuildEvent(string message)
            : base(BuildEventType.Information, BuildEventImportance.Low, message)
        {
            _progressSubject = new Subject<int>();
        }

        /// <summary>
        /// Allows the creator of this event to make the progress tick with a given percentage.
        /// </summary>
        /// <param name="percent">The progress percentage that this tick represents.</param>
        public void Tick(int percent)
        {
            _progressSubject.OnNext(percent);
        }
        /// <summary>
        /// Defines a method to release allocated resources.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Defines a method to release allocated resources.
        /// </summary>
        /// <param name="disposing">If we are already disposing or not.</param>
        [ExcludeFromCodeCoverage]
        private void Dispose(bool disposing)
        {
            if (_disposed || !disposing)
            {
                return;
            }

            if (_progressSubject != null)
            {
                _progressSubject.Dispose();
            }

            _disposed = true;
        }

        [ExcludeFromCodeCoverage]
        ~ProgressBuildEvent()
        {
            Dispose(false);
        }
    }
}

namespace DevOpsFlex.Core.Events
{
    /// <summary>
    /// Represents a build event that got fired by a build action that was called
    /// by a high level build activity, either a PowerShell commandlet or a TFS build activity.
    /// </summary>
    public class BuildEvent
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BuildEvent"/>.
        /// </summary>
        /// <param name="type">The <see cref="BuildEventType"/> of the event.</param>
        /// <param name="importance">The <see cref="BuildEventImportance"/> of the event.</param>
        /// <param name="message">The text message that describes the event.</param>
        public BuildEvent(BuildEventType type, BuildEventImportance importance, string message)
        {
            Type = type;
            Importance = importance;
            Message = message;
        }

        /// <summary>
        /// Gets the <see cref="BuildEventType"/> of the event.
        /// </summary>
        public BuildEventType Type { get; private set; }

        /// <summary>
        /// Gets the <see cref="BuildEventImportance"/> of the event.
        /// </summary>
        public BuildEventImportance Importance { get; private set; }

        /// <summary>
        /// Gets the text message that describes the event.
        /// </summary>
        public string Message { get; private set; }
    }

    /// <summary>
    /// Describes the type of build events that can happen.
    /// </summary>
    public enum BuildEventType
    {
        Information,
        Warning,
        Error
    }

    /// <summary>
    /// Describes the importance of build events.
    /// </summary>
    public enum BuildEventImportance
    {
        High,
        Medium,
        Low
    }
}

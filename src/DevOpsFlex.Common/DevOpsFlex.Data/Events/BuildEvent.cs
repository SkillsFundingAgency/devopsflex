﻿namespace DevOpsFlex.Data.Events
{
    using System.ComponentModel.DataAnnotations;

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

        [Key]
        public int Id { get; set; }

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
        [Required, MaxLength(500)]
        public string Message { get; private set; }
    }

    /// <summary>
    /// Describes the type of build events that can happen.
    /// </summary>
    public enum BuildEventType : short
    {
        Information,
        Warning,
        Error,
        Key
    }

    /// <summary>
    /// Describes the importance of build events.
    /// </summary>
    public enum BuildEventImportance : short
    {
        High,
        Medium,
        Low
    }
}

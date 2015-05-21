namespace DevOpsFlex.Data.Events
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents a build event that got fired after an ACL was created for a storage account.
    /// </summary>
    public class StorageKeyEvent : BuildEvent
    {
        /// <summary>
        /// Initializes a new instance of <see cref="StorageKeyEvent"/>.
        /// </summary>
        /// <param name="account">The name of the storage account that this key was created in.</param>
        /// <param name="container">The name of the storage container that this key was created in.</param>
        /// <param name="key">The value of this key (ACL).</param>
        public StorageKeyEvent(string account, string container, string key)
            : base(BuildEventType.Key, BuildEventImportance.High, $"Key created in account [{container}] for container [{account}]: {key}")
        {
            Account = account;
            Container = container;
            Key = key;
        }

        /// <summary>
        /// Gets the name of the storage account that this key was created in.
        /// </summary>
        [Required, MaxLength(100)]
        public string Account { get; private set; }

        /// <summary>
        /// Gets the name of the storage container that this key was created in.
        /// </summary>
        [Required, MaxLength(100)]
        public string Container { get; private set; }

        /// <summary>
        /// Gets the value of this key (ACL).
        /// </summary>
        [Required, MaxLength(100)]
        public string Key { get; private set; }
    }
}

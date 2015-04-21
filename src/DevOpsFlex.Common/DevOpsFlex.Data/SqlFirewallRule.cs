namespace DevOpsFlex.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics.Contracts;

    public class SqlFirewallRule
    {
        private long _rawExclusions;

        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("System")]
        public int SystemId { get; set; }
        public virtual DevOpsSystem System { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MinLength(7), MaxLength(15)]
        public string StartIp { get; set; }

        [Required, MinLength(7), MaxLength(15)]
        public string EndIp { get; set; }

        [Obsolete("You should not work with this directly, but instead with the generic SetExclusions and GetExclusions and push your own Enum.")]
        public long RawExclusions
        {
            get { return _rawExclusions; }
            set { _rawExclusions = value; }
        }

        /// <summary>
        /// Takes a generic <see cref="Enum"/> and uses it to populate the generic Exclusions flag.
        /// </summary>
        /// <param name="value">The flags <see cref="Enum"/>.</param>
        public void SetExclusions(Enum value)
        {
            Contract.Requires(value != null);
            Contract.Requires(Enum.GetUnderlyingType(value.GetType()) != typeof(UInt64));

            _rawExclusions = (long)Convert.ChangeType(value, typeof(long));
        }

        /// <summary>
        /// Populates a generic <see cref="Enum"/> with the Exclusion flags.
        /// </summary>
        /// <typeparam name="T">The concrete <see cref="Enum"/> type.</typeparam>
        /// <returns>The <see cref="Enum"/> type value with the Exclusion flags set.</returns>
        public T GetExclusions<T>() where T : struct
        {
            Contract.Requires(typeof(T).IsEnum);
            Contract.Requires(Enum.GetUnderlyingType(typeof(T)) != typeof(UInt64));

            return (T) (dynamic)_rawExclusions;
        }
    }
}

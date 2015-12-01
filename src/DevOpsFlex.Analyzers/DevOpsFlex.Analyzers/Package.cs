namespace DevOpsFlex.Analyzers
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Wraps logic around Name, Version and generic regular expression lazy initializations to support
    /// the package consolidation analyzer.
    /// </summary>
    public class Package : IEqualityComparer<Package>
    {
        private static readonly string PackageVersionRegex = PackagesFolderName.Replace("\\", "\\\\") + "[^0-9]*([0-9]+(?:\\.[0-9]+)+)(?:\\\\)?";
        private static readonly string PackageNameRegex = PackagesFolderName.Replace("\\", "\\\\") + "([a-zA-Z]+(?:\\.[a-zA-Z]+)*)[^\\\\]*(?:\\\\)?";
        private static readonly string PackageFolderRegex = "(.*" + PackagesFolderName.Replace("\\", "\\\\") + "[^\\\\]*)\\\\?";

        private string _version;
        private string _name;

        /// <summary>
        /// This is a convention constant that olds a string that all folders that we consider a "packages" folder contain.
        /// </summary>
        internal const string PackagesFolderName = "\\packages\\"; // convention

        /// <summary>
        /// Initializes a new instance of <see cref="Package"/>.
        /// Has built in Contract validations that will all throw before any other code is able to throw.
        /// </summary>
        /// <param name="path">The path to the package folder that this package is based on.</param>
        public Package(string path)
        {
            Contract.Requires(!string.IsNullOrEmpty(path));
            Contract.Requires(Directory.Exists(path));
            Contract.Requires(path.Contains(PackagesFolderName));
            Contract.Requires(Regex.IsMatch(path, PackageFolderRegex, RegexOptions.Singleline), $"When casting string (path) to Package you need to ensure your path is being matched by the Folder Regex [{PackageFolderRegex}]");

            Folder = Regex.Match(path, PackageFolderRegex, RegexOptions.Singleline).Groups[1].Value;
        }

        /// <summary>
        /// Gets the package folder without the last "\".
        /// </summary>
        public string Folder { get; }

        /// <summary>
        /// Gets the package name component of the package folder as a string.
        /// </summary>
        public string Name => _name ?? (_name = Regex.Match(Folder, PackageNameRegex, RegexOptions.Singleline).Groups[1].Value);

        /// <summary>
        /// Gets the package version component of the package folder as a string.
        /// </summary>
        public string Version => _version ?? (_version = Regex.Match(Folder, PackageVersionRegex, RegexOptions.Singleline).Groups[1].Value);

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first <see cref="Package"/> object to compare.</param>
        /// <param name="y">The second <see cref="Package"/> object to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(Package x, Package y)
        {
            Contract.Requires(x != null);
            Contract.Requires(y != null);

            return x.Folder == y.Folder;
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="package">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(Package package)
        {
            Contract.Requires(package != null);

            return package.Folder.GetHashCode();
        }
    }
}
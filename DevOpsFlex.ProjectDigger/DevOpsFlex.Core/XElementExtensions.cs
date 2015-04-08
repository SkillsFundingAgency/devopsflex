namespace DevOpsFlex.Core
{
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Contains extensions to the <see cref="XElement"/> class. Provides helper
    /// search methods for Linq2Xml.
    /// </summary>
    public static class XElementExtensions
    {
        /// <summary>
        /// Finds a chield <see cref="XElement"/> based on it's element name and on an attribute search criteria.
        /// </summary>
        /// <param name="root">The root of the XML <see cref="XElement"/> that we want to perform the search on.</param>
        /// <param name="elementName">The name of the element that we are looking for.</param>
        /// <param name="attributeName">The name of the attribute </param>
        /// <param name="contains"></param>
        /// <returns></returns>
        public static XElement FindElementThatContains(
            this XElement root,
            string elementName,
            string attributeName = null,
            string contains = null)
        {
            Contract.Requires(root != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(elementName));

            if (attributeName == null)
            {
                Contract.Assume(contains == null, "You can't call this with a null [attributeName] and a non null [contains] value. There's nothing to search for if [attributeName] is null.");

                return root.Descendants()
                           .FirstOrDefault(
                               e =>
                                   e.Name.LocalName == elementName);
            }

            Contract.Assume(!string.IsNullOrWhiteSpace(contains), "You need to search for something when you declare an attribute name. [contains] can't be null on a non-null [attributeName.]");

            return root.Descendants()
                       .FirstOrDefault(
                           e =>
                               e.Name.LocalName == elementName &&
                               e.Attributes().Any(a => a.Name.LocalName == attributeName) &&
                               e.Attributes()
                                .Single(a => a.Name.LocalName == attributeName)
                                .Value
                                .ToLowerInvariant()
                                .Contains(contains.ToLower()));
        }

        /// <summary>
        /// Gets the condition attribute for a given <see cref="XElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> that we want to get the attribute from.</param>
        /// <returns>The <see cref="XAttribute"/> if present, null otherwise.</returns>
        public static XAttribute GetConditionAttribute(this XElement element)
        {
            Contract.Requires(element != null);

            return element.Attributes()
                          .FirstOrDefault(a => a.Name.LocalName == "Condition");
        }

        /// <summary>
        /// Returns true if the given <see cref="XElement"/> is only declared for a specific configuration.
        /// Fase otherwise.
        /// </summary>
        /// <remarks>
        /// If the condition attribute isn't found, it will return false by default.
        /// </remarks>
        /// <param name="element">The <see cref="XElement"/> that we want to get the attribute from.</param>
        /// <param name="configuration">The project configuration value.</param>
        /// <returns>True if the given <see cref="XElement"/> is only declared for a specific configuration. Fase otherwise.</returns>
        public static bool IsConfigurationConditional(this XElement element, string configuration)
        {
            Contract.Requires(element != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(configuration));

            var att = element.GetConditionAttribute();
            if (att == null) return false;

            return att.Value
                      .ToLowerInvariant()
                      .Replace(" ", "")
                      .Contains("'$(configuration)'=='" + configuration.ToLowerInvariant() + "'");
        }
    }
}

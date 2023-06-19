namespace Siesta.Configuration.RequestConfiguration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Class that represents filter information for enumerable endpoints.
    /// This includes the required page number and size.
    /// </summary>
    public class EnumerableFilterInformation
    {
        /// <summary>
        /// Gets or sets the number of the page of Enumerable data to return.
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Gets or sets the page size to partition the Enumerable data into.
        /// </summary>
        public int PageSize { get; set; } = 25;

        /// <summary>
        /// Takes the filter information and transforms it into a dictionary to be used for HTTP query parameters.
        /// </summary>
        /// <returns>Dictionary with property name value pairs.</returns>
        public Dictionary<string, string> AsQueryDictionary()
        {
            var properties = this.GetType()
                .GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
            var dictionary = new Dictionary<string, string>();

            foreach (var property in properties)
            {
                var value = property.GetValue(this);
                if (value?.ToString() is not null)
                {
                    dictionary.Add(property.Name, Uri.EscapeDataString(value.ToString() !));
                }
            }

            return dictionary;
        }
    }
}

using System;
using System.Reflection;

namespace EasyData.Services
{
    /// <summary>
    /// Contains options for full text search
    /// </summary>
    public class FullTextSearchOptions
    {
        /// <summary>
        /// Lamda expression, which filters properties to use in full text search
        /// </summary>
        public Func<PropertyInfo, bool> Filter { get; set; }

        /// <summary>
        /// The name of the property to order by the result list
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// if set to <c>true</c> then we use descending order
        /// </summary>
        public bool IsDescendingOrder { get; set; } = false;

        /// <summary>
        /// Should the filter text match case.
        /// </summary>
        public bool MatchCase { get; set; }

        /// <summary>
        /// Should the filter text match whole word.
        /// </summary>
        public bool MatchWholeWord { get; set; }

        /// <summary>
        /// Depth of full text search. 
        /// </summary>
        public int Depth { get; set; } = 0;
    }
}

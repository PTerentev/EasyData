using System.Collections.Generic;

namespace EasyData.Services
{
    /// <summary>
    /// Substring filter options.
    /// </summary>
    public sealed class SubstringFilterOptions
    {
        /// <summary>
        /// Filter text.
        /// </summary>
        public string FilterText { get; set; }

        /// <summary>
        /// Should the filter text match case.
        /// </summary>
        public bool? MatchCase { get; set; }

        /// <summary>
        /// Should the filter text match whole word.
        /// </summary>
        public bool? MatchWholeWord { get; set; }

        /// <summary>
        /// Related objects included to the filter.
        /// </summary>
        public IReadOnlyCollection<string> RelatedObjectsToFilter { get; set; }
    }
}

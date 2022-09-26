using System;

namespace ElasticSearchSearchEnhancement.Models.SearchRelatedTemplates
{
    [Flags]
    public enum FilterTypes
    {
        /// <summary>
        /// Represents a text based filter.
        /// </summary>
        Text = 1,

        /// <summary>
        /// Represents a date range filter.
        /// </summary>
        DateRange = 2,

        /// <summary>
        /// Represent a not equal to filter.
        /// </summary>
        NotEqualTo = 3,

        /// <summary>
        /// Represents a contains filter.
        /// </summary>
        Contains = 4,

        /// <summary>
        /// Represent a not equal to filter.
        /// </summary>
        LessThanOrEqualToXDayFromToday = 5,

        /// <summary>
        /// Represent a match filter.
        /// </summary>
        Equals = 6,
    }
}

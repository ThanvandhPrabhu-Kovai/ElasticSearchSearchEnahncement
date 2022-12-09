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

        /// <summary>
        /// Represent greater than or equal to X number of days from today filter
        /// </summary>
        GreaterThanOrEqualToXDayFromToday = 7,

        /// <summary>
        /// Represent value that starts with input query.
        /// </summary>
        StartsWith = 8,

        /// <summary>
        /// Represent value that end with input query.
        /// </summary>
        EndsWith = 9,

        /// <summary>
        /// Represent values that falls within input range of numbers.
        /// </summary>
        Range = 10,

        /// <summary>
        /// Represent values that falls greater than input query.
        /// </summary>
        GreaterThan = 11,

        /// <summary>
        /// Represent value that falls less than input query.
        /// </summary>
        LessThan = 12,

        /// <summary>
        /// Represent value that falls greater than or equal to input query.
        /// </summary>
        GreaterThanOrEqualTo = 13,

        /// <summary>
        /// Represent value that falls lesser than or equal to input query.
        /// </summary>
        LessThanOrEqualTo = 14,

        /// <summary>
        /// Represent less than or equal to X number of days before today filter
        /// </summary>
        LessThanOrEqualToXDayBeforeToday = 15,

        /// <summary>
        /// Represent less than or equal to X number of days before today filter
        /// </summary>
        GreaterThanOrEqualToXDayBeforeToday = 16,
    }
}

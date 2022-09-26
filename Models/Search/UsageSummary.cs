using System;
using System.Collections.Generic;

namespace Kovai.Churn360.Customers.Core.Models
{
    public class UsageSummary
    {
        public DateTime FirstActivityOn { get; set; }

        public DateTime LastActivityOn { get; set; }

        public int TotalActiveUsers { get; set; }

        public int WeeklyActiveUsers { get; set; }

        public int MonthlyActiveUsers { get; set; }

        public ICollection<string> Features { get; set; }

        public ICollection<string> Browsers { get; set; }

        public ICollection<int> Countries { get; set; }
    }
}

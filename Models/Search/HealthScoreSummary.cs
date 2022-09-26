using System;
using System.Collections.Generic;

namespace Kovai.Churn360.Customers.Core.Models
{
    public class HealthScoreSummary
    {
        public int Score { get; set; }

        public int Health { get; set; }

        public int SeventhDayScoreDiff { get; set; }

        public int SeventhDayHealth { get; set; }

        public int ThirtythDayScoreDiff { get; set; }

        public int ThirtythDayHealth { get; set; }

        public int SixtythDayScoreDiff { get; set; }

        public int SixtythDayHealth { get; set; }

        public int OneEightyDayScoreDiff { get; set; }

        public int OneEightyDayHealth { get; set; }

        public int OneYearScoreDiff { get; set; }

        public int OneYearHealth { get; set; }
    }
}

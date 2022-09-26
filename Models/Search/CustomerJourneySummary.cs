using System;
using System.Collections.Generic;

namespace Kovai.Churn360.Customers.Core.Models
{
    public class CustomerJourneySummary
    {
        public int StageId { get; set; }

        public DateTime StartDayInStage { get; set; }
    }
}

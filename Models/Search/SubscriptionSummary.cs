using System;
using System.Collections.Generic;

namespace Kovai.Churn360.Customers.Core.Models
{
    public class SubscriptionSummary
    {
        public int Id { get; set; }

        public int? PlanId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int StatusId { get; set; }

        public decimal Mrr { get; set; }

        public decimal Arr { get; set; }

        public int? TotalLicenses { get; set; }

        public int? UsedLicenses { get; set; }

        public CreditCardSummary CreditCard { get; set; }

        public IEnumerable<InvoiceSummary> Invoices { get; set; }
    }
}

using System;

namespace Kovai.Churn360.Customers.Core.Models
{
    public class InvoiceSummary
    {
        public int Id { get; set; }

        public string Status { get; set; }

        public DateTime? DueDate { get; set; }

        public decimal? Amount { get; set; }

        public decimal? AmountPaid { get; set; }

        public decimal? AmountDue { get; set; }

        public string CurrencyCode { get; set; }
    }
}
using System;

namespace Kovai.Churn360.Customers.Core.Models
{
    public class CreditCardSummary
    {
        public int? ExpiryYear { get; set; }

        public byte? ExpiryMonth { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public string Number { get; set; }
    }
}
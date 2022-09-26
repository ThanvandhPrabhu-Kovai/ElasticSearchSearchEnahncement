namespace Kovai.Churn360.Customers.Core.Models
{
    public class SupportTicketSummary
    {
        public SupportTicketStatusSummary Total { get; set; }

        public SupportTicketStatusSummary Open { get; set; }

        public SupportTicketStatusSummary Closed { get; set; }

        public SupportTicketStatusSummary Overdue { get; set; }
    }
}

using Kovai.Churn360.Customers.Core.Models;
using Nest;
using QueryEditor.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchSearchEnhancement.Models.Search
{

    public class CustomerSearch : Editable
    {
        public CustomerSearch()
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string CustomerSource { get; set; }

        public string ExternalId { get; set; }

        public string CustomerSuccessManagerId { get; set; }

        public int? IndustryId { get; set; }

        public int? CountryId { get; set; }

        public int? TimeZoneId { get; set; }

        public string Domain { get; set; }

        public string CrmDomain { get; set; }

        public int Status { get; set; }

        public bool? IsNew { get; set; }

        public string Identifier { get; set; }

        public string CustomerAge { get; set; }

        public CustomerAccountSummary Account { get; set; }

        public IEnumerable<string> CustomTags { get; set; }

        public IEnumerable<CustomerContact> Contacts { get; set; }

        public IEnumerable<CustomerOpportunities> Opportunities { get; set; }

        public SupportTicketSummary SupportTickets { get; set; }

        public TaskSummary Tasks { get; set; }

        public CustomerSentimentSummary Sentiment { get; set; }

        public JoinField UsageJoinField { get; set; } 

        //public UsageSummary Usage { get; set; }

        //public ICollection<int> Segments { get; set; }

        //public CustomerJourneySummary CustomerJourney { get; set; }

        //public HealthScoreSummary HealthScore { get; set; }
    }
}

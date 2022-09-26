using System.Collections.Generic;

namespace Kovai.Churn360.Customers.Core.Models
{
    public class CustomerOpportunities
    {
        public CustomerOpportunities()
        {
        }

        public CustomerOpportunities(int id, string name, int contactId)
        {
            this.Id = id;
            this.Name = name;
            this.ContactId = contactId;
        }

        public CustomerOpportunities(CustomerOpportunities opportunities)
        {
            this.Id = opportunities.Id;
            this.Name = opportunities.Name;
            this.ContactId = (int)opportunities.ContactId;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int? ContactId { get; set; }

        public IEnumerable<string> CustomTags { get; set; }

#nullable enable
        public CustomerSearch? Customer { get; set; }
    }
}

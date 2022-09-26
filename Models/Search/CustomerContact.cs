using System.Collections.Generic;

namespace Kovai.Churn360.Customers.Core.Models
{
    public class CustomerContact
    {
        public CustomerContact()
        {
        }

        public CustomerContact(int contactId, CustomerContact contact)
        {
            this.MapContact(contact);
        }

        public CustomerContact(CustomerContact contact, CustomerSearch customerSearch)
        {
            this.MapContact(contact);
            this.Customer = customerSearch;
        }

        public CustomerContact(CustomerContact contact, IEnumerable<CustomerContactCampaign> campaigns)
        {
            this.MapContact(contact);
            this.Campaigns = campaigns;
        }

        private void MapContact(CustomerContact contact)
        {
            this.Id = contact.Id;
            this.FirstName = contact.FirstName;
            this.LastName = contact.LastName;
            this.Email = contact.Email;
            this.Phone = contact.Phone;
            this.CustomerId = (int)contact.CustomerId;
        }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public int CustomerId { get; set; }

        public string Shield { get; set; }

#nullable enable
        public CustomerSearch? Customer { get; set; }

        public IEnumerable<CustomerContactCampaign> Campaigns { get; set; }
    }
}

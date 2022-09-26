using QueryEditor.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kovai.Churn360.Customers.Core.Models
{
    public class DynamicCustomerConverter : JsonConverter<CustomerSearch>
    {
        public override CustomerSearch Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, CustomerSearch value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var kvp in value.Get())
            {
                writer.WriteString(kvp.Key, kvp.Value.ToString());
            }
            writer.WriteEndObject();
        }
    }

    public class CustomerSearch : DynamicObject
    {
        private Dictionary<string, object> _dictionary = new Dictionary<string, object> { };

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _dictionary.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            AddProperty(binder.Name, value);
            return true;
        }

        public Dictionary<string, object> Get() {
            return _dictionary;
        }

        public void AddProperty(string name, object value)
        {
            _dictionary[name] = value;
        }

        public object GetProperty(string name)
        {
            return _dictionary;
        }
    }

    //public class CustomerSearch : Editable
    //{
    //    public CustomerSearch()
    //    {
    //    }

    //    public int Id { get; set; }

    //    public string Name { get; set; }

    //    public string CustomerSource { get; set; }

    //    public string ExternalId { get; set; }

    //    public string CustomerSuccessManagerId { get; set; }

    //    public int? IndustryId { get; set; }

    //    public int? CountryId { get; set; }

    //    public int? TimeZoneId { get; set; }

    //    public string Domain { get; set; }

    //    public string CrmDomain { get; set; }

    //    public int Status { get; set; }

    //    public bool? IsNew { get; set; }

    //    public string Identifier { get; set; }

    //    public string CustomerAge { get; set; }

    //    public CustomerAccountSummary Account { get; set; }

    //    public IEnumerable<string> CustomTags { get; set; }

    //    public IEnumerable<CustomerContact> Contacts { get; set; }

    //    public IEnumerable<CustomerOpportunities> Opportunities { get; set; }

    //    public SupportTicketSummary SupportTickets { get; set; }

    //    public TaskSummary Tasks { get; set; }

    //    public CustomerSentimentSummary Sentiment { get; set; }

    //    ////public UsageSummary Usage { get; set; }

    //    ////public ICollection<int> Segments { get; set; }

    //    ////public CustomerJourneySummary CustomerJourney { get; set; }

    //    ////public HealthScoreSummary HealthScore { get; set; }
    ////}
}

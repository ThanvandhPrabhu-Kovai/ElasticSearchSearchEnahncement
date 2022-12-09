using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kovai.Churn360.Customers.Core.Models
{
    internal class CustomerSearchConverter : JsonConverter<DynamicResponse>
    {
        public override DynamicResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var customerSearch = new DynamicResponse();
            using (var jsonDoc = JsonDocument.ParseValue(ref reader))
            {
                var dictionary = jsonDoc.RootElement.EnumerateObject()
                                .ToDictionary(k => k.Name, v => v.Value.Clone());
                dictionary.Keys.ToList().ForEach((key) => {
                    customerSearch.SetProperty(key, dictionary[key]);
                });
            }
            return customerSearch;
        }

        public override void Write(Utf8JsonWriter writer, DynamicResponse value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var kvp in value.Get())
            {
                writer.WriteString(kvp.Key, kvp.Value.ToString());
            }
            writer.WriteEndObject();
        }
    }

    public class DynamicResponse : DynamicObject
    {
        private Dictionary<string, object> _dictionary = new Dictionary<string, object> { };

        public object this[string name]
        {
            get { return _dictionary[name]; }
            set { SetProperty(name, value); }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _dictionary.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetProperty(binder.Name, value);
            return true;
        }

        public Dictionary<string, object> Get() {
            return _dictionary;
        }

        public void SetProperty(string name, object value)
        {
            _dictionary[name] = value;
        }

        public object GetProperty(string name)
        {
            return _dictionary[name];
        }

        public string ToJson() {
            return JsonSerializer.Serialize(this._dictionary, new JsonSerializerOptions() {
                WriteIndented = true,
                Converters = {
                    new CustomerSearchConverter()
                 },
            });
        }

        public DynamicResponse FromJson(string json) {
            return JsonSerializer.Deserialize<DynamicResponse>(json, new JsonSerializerOptions() {
                Converters = {
                    new CustomerSearchConverter()
                 },
            });
        }
    }

}

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElasticSearchSearchEnhancement.Models
{
    public class DynamicCustomerConverter : JsonConverter<Customer>
    {
        public override Customer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Customer value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var kvp in value._dictionary) {
                writer.WriteString(kvp.Key, kvp.Value.ToString());
            }
            writer.WriteEndObject();
        }
    }

    public class Customer : DynamicObject
    {

        internal Dictionary<string, object> _dictionary = new Dictionary<string, object> { };

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _dictionary.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            AddProperty(binder.Name, value);
            return true;
        }

        public void AddProperty(string name, object value)
        {
            _dictionary[name] = value;
        }
    }
}

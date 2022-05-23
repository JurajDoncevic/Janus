using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions.JsonConversion
{
    public class SelectionExpressionJsonConverter : JsonConverter<SelectionExpression>
    {
        public override SelectionExpression? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var selectionExpression = JsonDocument.ParseValue(ref reader).Deserialize<SelectionExpression>();
            return selectionExpression;
        }

        public override void Write(Utf8JsonWriter writer, SelectionExpression value, JsonSerializerOptions options)
        {
            string stringRepresentation = value.ToString();
            
            writer.WriteRawValue(stringRepresentation);
        }

    }
}

using Janus.Commons.QueryModels.JsonConversion.DTOs;
using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels.JsonConversion
{
    public class QueryJsonConverter : JsonConverter<Query>
    {

        public override Query? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var queryDTO = JsonDocument.ParseValue(ref reader).Deserialize<QueryDTO>();

            if (queryDTO == null)
                throw new Exception("Deserialization of QueryDTO failed");

            var query =
                QueryModelOpenBuilder.InitOpenQuery(queryDTO.OnTableauId)
                    .WithJoining(conf => queryDTO.Joining != null
                                         ? queryDTO.Joining.Fold(conf, (j, c) => conf.AddJoin(j.ForeignKeyAttributeId, j.PrimaryKeyAttributeId))
                                         : conf)
                    .WithSelection(conf => queryDTO.Selection != null
                                            ? conf.WithExpression(queryDTO.Selection.Expression)
                                            : conf)
                    .WithProjection(conf => queryDTO.Projection != null
                                            ? queryDTO.Projection.AttributeIds.Fold(conf, (attrId, c) => c.AddAttribute(attrId))
                                            : conf)
                    .Build();

            return query;
        }

        public override void Write(Utf8JsonWriter writer, Query value, JsonSerializerOptions options)
        {
            var queryDTO = new QueryDTO
            {
                OnTableauId = value.OnTableauId,
                Joining =
                    value.Joining
                         .Map(joining => joining.Joins)
                         .Map(joins => joins.Map(join =>
                              new JoinDTO()
                              {
                                  ForeignKeyAttributeId = join.ForeignKeyAttributeId,
                                  PrimaryKeyAttributeId = join.PrimaryKeyAttributeId,
                                  ForeignKeyTableauId = join.ForeignKeyTableauId,
                                  PrimaryKeyTableauId = join.PrimaryKeyTableauId
                              }).ToList()).Value, // can be null, but don't care for DTO
                Projection =
                    value.Projection
                         .Map(p => new ProjectionDTO { AttributeIds = p.IncludedAttributeIds.ToHashSet() })
                         .Value, // can be null, but don't care for DTO
                Selection =
                    value.Selection
                         .Map(s => new SelectionDTO { Expression = s.Expression })
                         .Value

            };

            var json = JsonSerializer.Serialize<QueryDTO>(queryDTO);
            writer.WriteRawValue(json);
        }
    }
}

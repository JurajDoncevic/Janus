using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Results;
using Janus.Commons.QueryModels;
using Janus.Serialization.Bson.QueryModels.DTOs;
using System.Text;
using System.Text.Json;

namespace Janus.Serialization.Bson.QueryModels;

/// <summary>
/// BSON format query serializer
/// </summary>
public class QuerySerializer : IQuerySerializer<byte[]>
{
    private readonly JsonSerializerOptions _serializerOptions;

    public QuerySerializer()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new SelectionExpressionJsonConverter());

        _serializerOptions = options;
    }

    /// <summary>
    /// Deserializes a query
    /// </summary>
    /// <param name="serialized">Serialized query</param>
    /// <returns>Deserialized query</returns>
    public Result<Query> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() => JsonSerializer.Deserialize<QueryDto>(serialized, _serializerOptions) ?? throw new Exception("Failed to deserialize QueryDTO"))
            .Bind(FromDto);

    /// <summary>
    /// Serializes a query
    /// </summary>
    /// <param name="query">Query to serialize</param>
    /// <returns>Serialized query</returns>
    public Result<byte[]> Serialize(Query query)
        => ResultExtensions.AsResult(()
            => ToDto(query)
                .Map(queryDto => JsonSerializer.Serialize(queryDto, _serializerOptions))
                .Map(Encoding.UTF8.GetBytes));

    /// <summary>
    /// Converts a query to its DTO
    /// </summary>
    /// <param name="query">Query model</param>
    /// <returns>Query DTO</returns>
    internal Result<QueryDto> ToDto(Query query)
        => ResultExtensions.AsResult(() =>
        {
            var queryDto = new QueryDto
            {
                OnTableauId = query.OnTableauId,
                Joining =
                    query.Joining
                         .Map(joining => joining.Joins)
                         .Map(joins => joins.Map(join =>
                              new JoinDto()
                              {
                                  ForeignKeyAttributeId = join.ForeignKeyAttributeId,
                                  PrimaryKeyAttributeId = join.PrimaryKeyAttributeId,
                                  ForeignKeyTableauId = join.ForeignKeyTableauId,
                                  PrimaryKeyTableauId = join.PrimaryKeyTableauId
                              }).ToList()).Value, // can be null, but don't care for DTO
                Projection =
                    query.Projection
                         .Map(p => new ProjectionDto { AttributeIds = p.IncludedAttributeIds.ToHashSet() })
                         .Value, // can be null, but don't care for DTO
                Selection =
                    query.Selection
                         .Map(s => new SelectionDto { Expression = s.Expression })
                         .Value

            };

            return queryDto;
        });

    /// <summary>
    /// Converts a query DTO to the query model
    /// </summary>
    /// <param name="queryDto">Query DTO</param>
    /// <returns>Query model</returns>
    internal Result<Query> FromDto(QueryDto queryDto)
        => ResultExtensions.AsResult(() =>
        {
            var query =
                QueryModelOpenBuilder.InitOpenQuery(queryDto.OnTableauId)
                    .WithJoining(conf => queryDto.Joining != null
                                         ? queryDto.Joining.Fold(conf, (j, c) => conf.AddJoin(j.ForeignKeyAttributeId, j.PrimaryKeyAttributeId))
                                         : conf)
                    .WithSelection(conf => queryDto.Selection != null
                                            ? conf.WithExpression(queryDto.Selection.Expression)
                                            : conf)
                    .WithProjection(conf => queryDto.Projection != null
                                            ? queryDto.Projection.AttributeIds.Fold(conf, (attrId, c) => c.AddAttribute(attrId))
                                            : conf)
                    .Build();

            return query;
        });
}

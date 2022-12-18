using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Serialization.Protobufs.DataModels.DTOs;
using System.Text;

namespace Janus.Serialization.Protobufs.DataModels;

/// <summary>
/// Protobufs format serializer for tabular data
/// </summary>
public sealed class TabularDataSerializer : ITabularDataSerializer<byte[]>
{

    /// <summary>
    /// Deserializes tabular data
    /// </summary>
    /// <param name="serialized">Serialized tabular data</param>
    /// <returns>Deserialized tabular data</returns>
    public Result<TabularData> Deserialize(byte[] serialized)
        => Results.AsResult(() => Utils.FromProtobufs<TabularDataDto>(serialized))
            .Bind(FromDto);

    /// <summary>
    /// Serializes tabular data
    /// </summary>
    /// <param name="data">Tabular data to serialize</param>
    /// <returns>Serialized tabular data</returns>
    public Result<byte[]> Serialize(TabularData data)
        => Results.AsResult(()
            => ToDto(data)
               .Map(tabularDataDto => Utils.ToProtobufs(tabularDataDto))
            );

    /// <summary>
    /// Converts tabular data to its DTO
    /// </summary>
    /// <param name="tabularData">Tabular data model</param>
    /// <returns>tabular data DTO</returns>
    internal Result<TabularDataDto> ToDto(TabularData tabularData)
        => Results.AsResult(() =>
        {
            var tabularDataDto = new TabularDataDto
            { 
                Name = tabularData.Name,
                AttributeDataTypes = tabularData.ColumnDataTypes.ToDictionary(kv => kv.Key, kv => kv.Value),
                AttributeValues = tabularData.RowData
                                             .Select(rowData => new RowValuesDto { RowValues = rowData.ColumnValues.ToDictionary(av => av.Key, av => new DataBytesDto { Data = ConvertToBytes(av.Value, av.Value?.GetType() ?? typeof(object)) }) })
                                             .ToList()
            };

            return tabularDataDto;
        });

    /// <summary>
    /// Converts a tabular data DTO to a data model
    /// </summary>
    /// <param name="tabularDataDto">Tabular data DTO</param>
    /// <returns>Tabular data model</returns>
    internal Result<TabularData> FromDto(TabularDataDto tabularDataDto)
        => Results.AsResult(() =>
        {
            var tabularData =
            tabularDataDto.AttributeValues.Fold(
                        TabularDataBuilder.InitTabularData(tabularDataDto.AttributeDataTypes)
                                          .WithName(tabularDataDto.Name),
                        (attrVals, builder) => builder.AddRow(
                            conf => conf.WithRowData(attrVals.RowValues.ToDictionary(
                                av => av.Key,
                                av => av.Value != null
                                      ? ConvertFromBytes(av.Value.Data, TypeMappings.MapToType(tabularDataDto.AttributeDataTypes[av.Key]))
                                      : null
                                )))
                        ).Build();

            return tabularData;
        });

    /// <summary>
    /// Converts primitive data to a byte array
    /// </summary>
    /// <param name="value"></param>
    /// <param name="originalType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private byte[]? ConvertToBytes(object? value, Type originalType)
        => value == null ? Array.Empty<byte>() : originalType switch
        {
            Type t when t == typeof(int) => BitConverter.GetBytes((int)value),
            Type t when t == typeof(long) => BitConverter.GetBytes((long)value),
            Type t when t == typeof(double) => BitConverter.GetBytes((double)value),
            Type t when t == typeof(bool) => BitConverter.GetBytes((bool)value),
            Type t when t == typeof(DateTime) => BitConverter.GetBytes(DateTime.Now.Ticks),
            Type t when t == typeof(string) => Encoding.UTF8.GetBytes(value.ToString()),
            Type t when t == typeof(byte[]) => (byte[])value,
            _ => throw new ArgumentException($"No mapping for Type {originalType.FullName}")
        };

    /// <summary>
    /// Converts byte arrays to primitive data
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="expectedType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private object? ConvertFromBytes(byte[] bytes, Type expectedType)
        => bytes.Length == 0 ? null : expectedType switch
        {
            Type t when t == typeof(int) => BitConverter.ToInt32(bytes),
            Type t when t == typeof(long) => BitConverter.ToInt64(bytes),
            Type t when t == typeof(double) => BitConverter.ToDouble(bytes),
            Type t when t == typeof(bool) => BitConverter.ToBoolean(bytes),
            Type t when t == typeof(DateTime) => new DateTime(BitConverter.ToInt64(bytes)),
            Type t when t == typeof(string) => Encoding.UTF8.GetString(bytes),
            Type t when t == typeof(byte[]) => (byte[])bytes,
            _ => throw new ArgumentException($"No mapping for Type {expectedType.FullName}")
        };
}

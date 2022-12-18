using Janus.Commons.SchemaModels;
using System.ComponentModel;

namespace Janus.Commons.DataModels;

public static class TypeMappings
{
    /// <summary>
    /// Maps a <see cref="DataTypes"/> value to a <see cref="Type"/>
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static Type MapToType(DataTypes dataType)
        => dataType switch
        {
            DataTypes.INT => typeof(int),
            DataTypes.DECIMAL => typeof(double),
            DataTypes.BOOLEAN => typeof(bool),
            DataTypes.DATETIME => typeof(DateTime),
            DataTypes.STRING => typeof(string),
            DataTypes.BINARY => typeof(byte[]),
            _ => throw new ArgumentException($"No mapping for DataType {dataType}")
        };

    /// <summary>
    /// Maps a <see cref="Type"/> to a <see cref="DataTypes"/> value
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static DataTypes MapToDataType(Type type)
        => type switch
        {
            Type t when t == typeof(int) => DataTypes.INT,
            Type t when t == typeof(double) => DataTypes.DECIMAL,
            Type t when t == typeof(bool) => DataTypes.BOOLEAN,
            Type t when t == typeof(DateTime) => DataTypes.DATETIME,
            Type t when t == typeof(string) => DataTypes.STRING,
            Type t when t == typeof(byte[]) => DataTypes.BINARY,
            _ => throw new ArgumentException($"No mapping for Type {type.FullName}")
        };

    /// <summary>
    /// Checks if a System.Type can be represented as a given DataType
    /// </summary>
    /// <param name="type"></param>
    /// <param name="dataType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool IsRepresentableAs(this Type type, DataTypes dataType)
        => dataType switch
        {
            DataTypes.INT => type.IsEquivalentTo(typeof(int)),
            DataTypes.DECIMAL => type.IsEquivalentTo(typeof(double)),
            DataTypes.BOOLEAN => type.IsEquivalentTo(typeof(bool)),
            DataTypes.DATETIME => type.IsEquivalentTo(typeof(DateTime)),
            DataTypes.STRING => type.IsEquivalentTo(typeof(string)),
            DataTypes.BINARY => type.IsEquivalentTo(typeof(byte[])),
            _ => throw new ArgumentException($"No representation for DataType {dataType}")
        };
}

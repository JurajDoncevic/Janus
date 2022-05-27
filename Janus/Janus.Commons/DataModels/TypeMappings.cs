using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.DataModels
{
    public static class TypeMappings
    {
        public static Type MapToType(DataTypes dataType)
            => dataType switch
            {
                DataTypes.INT => typeof(int),
                DataTypes.DECIMAL => typeof(float),
                DataTypes.BOOLEAN => typeof(bool),
                DataTypes.DATETIME => typeof(DateTime),
                DataTypes.STRING => typeof(string),
                DataTypes.BINARY => typeof(byte[]),
                _ => throw new ArgumentException($"No mapping for DataType {dataType}")
            };

        public static DataTypes MapToDataType(Type type)
            => type switch
            {
                Type t when t == typeof(int) => DataTypes.INT,
                Type t when t == typeof(float) => DataTypes.DECIMAL,
                Type t when t == typeof(bool) => DataTypes.BOOLEAN,
                Type t when t == typeof(DateTime) => DataTypes.DATETIME,
                Type t when t == typeof(string) => DataTypes.STRING,
                Type t when t == typeof(byte[]) => DataTypes.BINARY,
                _ => throw new ArgumentException($"No mapping for Type {type.FullName}")
            };
    }
}

using Janus.Commons.DataModels;
using Janus.Wrapper.Sqlite.LocalDataModel;
using Janus.Wrapper.Sqlite.Translation;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using Xunit;

namespace Janus.Wrapper.Sqlite.Tests.Translation;
public class SqliteDataTranslationTests
{
    private readonly string _connectionString = "Data Source=./chinook.db";
    private readonly string _dataSourceName = "chinook";

    [Fact(DisplayName = "Translate data coming from multiple joined Sqlite tables")]
    public void TranslateMultiTableDataTest()
    {
        var queryText = "SELECT tracks.Name, albums.Title, genres.Name, artists.Name FROM tracks INNER JOIN artists ON artists.ArtistId=albums.ArtistId  INNER JOIN albums ON albums.AlbumId=tracks.AlbumId  INNER JOIN genres ON genres.GenreId=tracks.GenreId WHERE true;";

        using var connection = new SqliteConnection(_connectionString);
        if (connection.State == System.Data.ConnectionState.Closed)
            connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = queryText;

        using var reader = command.ExecuteReader();

        var dataRows = new List<Dictionary<string, object?>>(); 
        var dataSchema = new Dictionary<string, Type>();
        while (reader.Read())
        {
            if (dataSchema.Count == 0)
            {
                foreach (var column in reader.GetColumnSchema())
                {
                    dataSchema.Add($"{column.BaseCatalogName}.{column.BaseTableName}.{column.ColumnName}", column.DataType!);
                }
            }

            var row = new Dictionary<string, object?>();
            foreach (var columnName in dataSchema.Keys)
            {
                var columnIndex = dataSchema.Keys.ToList().IndexOf(columnName);
                var fieldValue = ReadFieldWithType(reader, columnIndex, dataSchema[columnName]);
                row.Add(columnName, fieldValue);
            }

            dataRows.Add(row);
        }

        var sqliteTabularData = new SqliteTabularData(dataSchema, dataRows);

        var dataTranslator = new SqliteDataTranslator(_dataSourceName);

        var translationResult = dataTranslator.Translate(sqliteTabularData);

        var tabularData = translationResult.Data!;

        Assert.True(translationResult);
        Assert.Equal(sqliteTabularData.DataSchema.ToDictionary(kv => $"{_dataSourceName}.{kv.Key}", kv => kv.Value), tabularData.ColumnDataTypes.ToDictionary(kv => kv.Key, kv => TypeMappings.MapToType(kv.Value)));
        Assert.Equal(3503, tabularData.RowData.Count);
    }

    /// <summary>
    /// Reads field value with provided type. Boxes data into object?
    /// </summary>
    /// <param name="reader">Open reader</param>
    /// <param name="columnIndex">Index of column to read</param>
    /// <param name="expectedType">Expected type in the column</param>
    /// <returns></returns>
    private object? ReadFieldWithType(SqliteDataReader reader, int columnIndex, Type expectedType)
        => reader.IsDBNull(columnIndex)
            ? null
            : expectedType switch
            {
                Type type when type.Equals(typeof(long)) => reader.GetInt64(columnIndex),
                Type type when type.Equals(typeof(double)) => reader.GetDouble(columnIndex),
                Type type when type.Equals(typeof(string)) => reader.GetString(columnIndex),
                Type type when type.Equals(typeof(byte[])) => (object)reader.GetFieldValue<byte[]>(columnIndex),
                Type type when type.Equals(typeof(DateTime)) => reader.GetDateTime(columnIndex),
                Type type when type.Equals(typeof(bool)) => reader.GetBoolean(columnIndex),
                _ => reader.GetDouble(columnIndex)
            };

    /// <summary>
    /// Gets a System.Type for a given Sqlite data type name. See more: https://www.sqlite.org/datatype3.html
    /// </summary>
    /// <param name="dataTypeName">Sqlite data type name</param>
    /// <returns>System.Type corresponding to the data type name</returns>
    private Type GetColumnTypeByDataTypeName(string dataTypeName)
        => dataTypeName switch
        {
            string dtn when dtn.Contains("INT") => typeof(long),
            string dtn when dtn.Contains("REAL") || dtn.Contains("FLOA") || dtn.Contains("DOUB") => typeof(double),
            string dtn when dtn.Contains("CHAR") || dtn.Contains("CLOB") || dtn.Contains("TEXT") => typeof(string),
            string dtn when dtn.Contains("BLOB") => typeof(byte[]),
            string dtn when dtn.Contains("DATE") => typeof(DateTime),
            string dtn when dtn.Contains("BOOL") => typeof(bool),
            _ => typeof(double)
        };
}

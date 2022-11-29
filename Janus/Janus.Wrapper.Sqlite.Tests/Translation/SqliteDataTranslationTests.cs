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

        var results = new List<Dictionary<string, (Type, object)>>(); // {column name, (type, value)} 
        var rowSchema = new Dictionary<string, Type>();
        while (reader.Read())
        {
            if (rowSchema.Count == 0)
            {
                foreach (var column in reader.GetColumnSchema())
                {
                    rowSchema.Add($"{column.BaseCatalogName}.{column.BaseTableName}.{column.ColumnName}", column.DataType!);
                }
            }

            var row = new Dictionary<string, (Type, object)>();
            foreach (var columnName in rowSchema.Keys)
            {
                row.Add(columnName, (rowSchema[columnName], reader.GetValue(rowSchema.Keys.ToList().IndexOf(columnName))));
            }

            results.Add(row);
        }

        var sqliteTabularData = new SqliteTabularData { Data = results };

        var dataTranslator = new SqliteDataTranslator(_dataSourceName);

        var translationResult = dataTranslator.TranslateToTabularData(sqliteTabularData);

        var tabularData = translationResult.Data!;

        Assert.True(translationResult);
        Assert.Equal(sqliteTabularData.DataSchema.ToDictionary(kv => $"{_dataSourceName}.{kv.Key}", kv => kv.Value), tabularData.ColumnDataTypes.ToDictionary(kv => kv.Key, kv => TypeMappings.MapToType(kv.Value)));
        Assert.Equal(3503, tabularData.RowData.Count);
    }
}

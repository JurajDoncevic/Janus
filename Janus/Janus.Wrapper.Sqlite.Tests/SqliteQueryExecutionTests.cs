using FunctionalExtensions.Base.Resulting;
using Janus.Commons.QueryModels;
using Janus.Wrapper.SchemaInferrence;
using Janus.Wrapper.Sqlite.LocalQuerying;
using Janus.Wrapper.Sqlite.SchemaInferrence;
using Janus.Wrapper.Sqlite.Translation;
using Xunit;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Wrapper.Sqlite.Tests;
public class SqliteQueryExecutionTests
{
    private readonly string _connectionString = "Data Source=./chinook.db";
    private readonly string _dataSourceName = "chinook";

    [Fact(DisplayName = "Execute query over Sqlite chinook data source")]
    public async void ChinookQueryExecutionTest()
    {
        var dataSource =
                new SchemaInferrer(new SqliteSchemaModelProvider(_connectionString), "chinook")
                    .InferSchemaModel().Data!;

        var queryTranslator = new SqliteQueryTranslator();
        var queryExecutor = new SqliteQueryExecutor(_connectionString);
        var dataTranslator = new SqliteDataTranslator(_dataSourceName);
        var query = QueryModelBuilder.InitQueryOnDataSource("chinook.main.tracks", dataSource)
            .WithJoining(conf => conf.AddJoin("chinook.main.tracks.AlbumId", "chinook.main.albums.AlbumId")
                                     .AddJoin("chinook.main.albums.ArtistId", "chinook.main.artists.ArtistId")
                                     .AddJoin("chinook.main.tracks.GenreId", "chinook.main.genres.GenreId"))
            .WithSelection(conf => conf.WithExpression(TRUE()))
            .WithProjection(conf => conf.AddAttribute("chinook.main.tracks.Name")
                                        .AddAttribute("chinook.main.albums.Title")
                                        .AddAttribute("chinook.main.genres.Name")
                                        .AddAttribute("chinook.main.artists.Name"))
            .Build();

        var queryExecutionResult =
            (await Task.FromResult(queryTranslator.Translate(query))
                .Bind(queryExecutor.ExecuteQuery))
                .Bind(dataTranslator.Translate);

        Assert.True(queryExecutionResult);
        Assert.Equal(3503, queryExecutionResult.Data!.RowData.Count);
    }
}

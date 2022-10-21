using Janus.Commons.QueryModels;
using Janus.Wrapper.SchemaInferrence;
using Janus.Wrapper.Sqlite.SchemaInferrence;
using Janus.Wrapper.Sqlite.Translation;
using Xunit;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Wrapper.Sqlite.Tests.Translation;
public class SqliteQueryTranslationTests
{
    [Fact(DisplayName = "Translate a query with all clauses")]
    public void TranslateSimpleQueryTest()
    {
        var queryTranslator = new SqliteQueryTranslator();
        var schemaInferrer = new SchemaInferrer(new SqliteSchemaModelProvider("Data Source=./chinook.db;"), "chinook");

        var dataSource = schemaInferrer.InferSchemaModel().Data!;

        var expectedSqliteQueryText =
            "SELECT albums.Title, artists.Name " +
            "FROM albums INNER JOIN artists ON artists.ArtistId=albums.ArtistId " +
            "WHERE (((artists.Name=\"Hans Zimmer\" OR albums.Title=\"Monteverdi: L'Orfeo\") OR artists.Name=\"U2\") OR artists.ArtistId>=180);";

        var query = QueryModelBuilder.InitQueryOnDataSource("chinook.main.albums", dataSource)
            .WithJoining(conf => conf.AddJoin("chinook.main.albums.ArtistId", "chinook.main.artists.ArtistId"))
            .WithSelection(conf => conf.WithExpression(OR(OR(OR(EQ("chinook.main.artists.Name", "Hans Zimmer"), EQ("chinook.main.albums.Title", "Monteverdi: L'Orfeo")), EQ("chinook.main.artists.Name", "U2")), GE("chinook.main.artists.ArtistId", 180))))
            .WithProjection(conf => conf.AddAttribute("chinook.main.albums.Title")
                                        .AddAttribute("chinook.main.artists.Name"))
            .Build();

        var translationResult = queryTranslator.Translate(query);

        var sqliteQuery = translationResult.Data!;
        var sqliteQueryText = sqliteQuery.ToText();


        Assert.True(translationResult);
        Assert.Equal(expectedSqliteQueryText, sqliteQueryText);

    }

    [Fact(DisplayName = "Translate a query with multiple joins")]
    public void TranslateQueryWithMultipleJoinsTest()
    {
        var queryTranslator = new SqliteQueryTranslator();
        var schemaInferrer = new SchemaInferrer(new SqliteSchemaModelProvider("Data Source=./chinook.db;"), "chinook");

        var dataSource = schemaInferrer.InferSchemaModel().Data!;

        var expectedSqliteQueryText =
            "SELECT tracks.Name, albums.Title, genres.Name, artists.Name " +
            "FROM tracks " +
            "INNER JOIN artists ON artists.ArtistId=albums.ArtistId " +
            "INNER JOIN albums ON albums.AlbumId=tracks.AlbumId " +
            "INNER JOIN genres ON genres.GenreId=tracks.GenreId " +
            "WHERE true;";

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

        var translationResult = queryTranslator.Translate(query);

        var sqliteQuery = translationResult.Data!;
        var sqliteQueryText = sqliteQuery.ToText();


        Assert.True(translationResult);
        Assert.Equal(expectedSqliteQueryText, sqliteQueryText);
    }
}

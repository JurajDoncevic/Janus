using Janus.Wrapper.SchemaInferrence;
using Janus.Wrapper.Sqlite.SchemaInferrence;
using Xunit;

namespace Janus.Wrapper.Sqlite.Tests;
public class SqliteSchemaInferrenceTests
{
    private readonly SchemaInferrer _schemaInferrer;
    private readonly string[] _connectionStrings
        = new string[] { "Data Source=./chinook.db;", "Data Source=./usgs-lower-us.db;" };
    public SqliteSchemaInferrenceTests()
    {
        _schemaInferrer = new SchemaInferrer(new SqliteSchemaModelProvider(_connectionStrings[1]), "chinook");
    }

    [Fact]
    public void InferTestDbSchemaTest()
    {
        var expectedSchemaString = "(testdb ((main ((albums ((AlbumId INT 0 False True)(Title STRING 1 False False)(ArtistId INT 2 False False)))(artists ((ArtistId INT 0 False True)(Name STRING 1 True False)))(customers ((CustomerId INT 0 False True)(FirstName STRING 1 False False)(LastName STRING 2 False False)(Company STRING 3 True False)(Address STRING 4 True False)(City STRING 5 True False)(State STRING 6 True False)(Country STRING 7 True False)(PostalCode STRING 8 True False)(Phone STRING 9 True False)(Fax STRING 10 True False)(Email STRING 11 False False)(SupportRepId INT 12 True False)))(employees ((EmployeeId INT 0 False True)(LastName STRING 1 False False)(FirstName STRING 2 False False)(Title STRING 3 True False)(ReportsTo INT 4 True False)(BirthDate DATETIME 5 True False)(HireDate DATETIME 6 True False)(Address STRING 7 True False)(City STRING 8 True False)(State STRING 9 True False)(Country STRING 10 True False)(PostalCode STRING 11 True False)(Phone STRING 12 True False)(Fax STRING 13 True False)(Email STRING 14 True False)))(genres ((GenreId INT 0 False True)(Name STRING 1 True False)))(invoices ((InvoiceId INT 0 False True)(CustomerId INT 1 False False)(InvoiceDate DATETIME 2 False False)(BillingAddress STRING 3 True False)(BillingCity STRING 4 True False)(BillingState STRING 5 True False)(BillingCountry STRING 6 True False)(BillingPostalCode STRING 7 True False)(Total DECIMAL 8 False False)))(invoice_items ((InvoiceLineId INT 0 False True)(InvoiceId INT 1 False False)(TrackId INT 2 False False)(UnitPrice DECIMAL 3 False False)(Quantity INT 4 False False)))(media_types ((MediaTypeId INT 0 False True)(Name STRING 1 True False)))(playlists ((PlaylistId INT 0 False True)(Name STRING 1 True False)))(playlist_track ((PlaylistId INT 0 False True)(TrackId INT 1 False True)))(tracks ((TrackId INT 0 False True)(Name STRING 1 False False)(AlbumId INT 2 True False)(MediaTypeId INT 3 False False)(GenreId INT 4 True False)(Composer STRING 5 True False)(Milliseconds INT 6 False False)(Bytes INT 7 True False)(UnitPrice DECIMAL 8 False False)))))))";
        
        var inferrenceResult = _schemaInferrer.InferSchemaModel();
        var retrivedSchemaString = inferrenceResult.Data!.ToString().Replace("\n", "");

        Assert.True(inferrenceResult);
        Assert.Equal(expectedSchemaString, retrivedSchemaString);
    }
}

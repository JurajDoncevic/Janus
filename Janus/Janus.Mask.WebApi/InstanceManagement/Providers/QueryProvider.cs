using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Mask.WebApi.Lenses;
using Janus.Mask.WebApi.InstanceManagement.Templates;
using Janus.Mask.WebApi.Translation;

namespace Janus.Mask.WebApi.InstanceManagement.Providers;
public class QueryProvider<TId, TModel> where TModel : BaseDto
{
    private readonly FakeQueryManager _queryManager;

    private readonly TableauId _targetTableauId;
    private readonly AttributeId _identityAttributeId;

    public QueryProvider(TableauId targetTableauId, AttributeId indetityAttributeId)
    {
        _queryManager = new FakeQueryManager();
        _targetTableauId = targetTableauId;
        _identityAttributeId = indetityAttributeId;
    }


    public Result<TModel> Get(TId id)
    {
        string routeQuery = $"?{_identityAttributeId.AttributeName}={id}"; // leave the translator to do all the work :)
        var translatedQuery = QueryTranslation.TranslateQuery(_targetTableauId, routeQuery);

        var lens = new TabularDataObjectLens<TModel>(_targetTableauId.ToString() + ".");

        var queryResult =
            _queryManager.RunQuery(translatedQuery)
                .Map(data => lens.Get(data))
                .Map(data => data.FirstOrDefault());

        return queryResult;
    }

    public Result<IEnumerable<TModel>> GetAll(string? selection = null)
    {
        var lens = new TabularDataObjectLens<TModel>(_targetTableauId.ToString() + ".");
        var translatedQuery = QueryTranslation.TranslateQuery(_targetTableauId, selection);

        var queryResult =
            _queryManager.RunQuery(translatedQuery)
                .Map(data => lens.Get(data));

        return queryResult;
    }

    private class FakeQueryManager
    {
        public FakeQueryManager() { }
        public Result<TabularData> RunQuery(Query query)
        {
            var resultData =
            TabularDataBuilder.InitTabularData(new Dictionary<string, DataTypes>()
            {
                { "TestDataSource.other.Buildings.Id", DataTypes.INT },
                { "TestDataSource.other.Buildings.Name", DataTypes.STRING },
                { "TestDataSource.other.Buildings.StreetAddress", DataTypes.STRING },
                { "TestDataSource.other.Buildings.NumberOfStories", DataTypes.INT}
            }).AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
            {
                { "TestDataSource.other.Buildings.Id", 1 },
                { "TestDataSource.other.Buildings.Name", "Building A" },
                { "TestDataSource.other.Buildings.StreetAddress", "Unska 3A" },
                { "TestDataSource.other.Buildings.NumberOfStories", 3 }
            })).AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
            {
                { "TestDataSource.other.Buildings.Id", 2 },
                { "TestDataSource.other.Buildings.Name", "Building B" },
                { "TestDataSource.other.Buildings.StreetAddress", "Unska 3B" },
                { "TestDataSource.other.Buildings.NumberOfStories", 2 }
            })).AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
            {
                { "TestDataSource.other.Buildings.Id", 3 },
                { "TestDataSource.other.Buildings.Name", "Building C" },
                { "TestDataSource.other.Buildings.StreetAddress", "Unska 3C" },
                { "TestDataSource.other.Buildings.NumberOfStories", 13 }
            })).AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
            {
                { "TestDataSource.other.Buildings.Id", 4 },
                { "TestDataSource.other.Buildings.Name", "Building D" },
                { "TestDataSource.other.Buildings.StreetAddress", "Unska 3D" },
                {"TestDataSource.other.Buildings.NumberOfStories", 3 }
            })).Build();

            return Results.OnSuccess<TabularData>(resultData);
        }
    }
}

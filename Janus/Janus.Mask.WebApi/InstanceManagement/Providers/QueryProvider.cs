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
    private readonly MaskQueryManager _queryManager;
    private readonly WebApiQueryTranslator _queryTranslator;

    private readonly TableauId _targetTableauId;
    private readonly AttributeId _identityAttributeId;

    internal QueryProvider(TableauId targetTableauId, AttributeId indetityAttributeId, MaskQueryManager queryManager, WebApiQueryTranslator queryTranslator)
    {
        _queryManager = queryManager;
        _targetTableauId = targetTableauId;
        _identityAttributeId = indetityAttributeId;
        _queryTranslator = queryTranslator;
    }


    public Result<TModel> Get(TId id)
    {
        string routeQuery = $"?{_identityAttributeId.AttributeName}={id}"; // leave the translator to do all the work :)

        var lens = new TabularDataObjectLens<TModel>(_targetTableauId.ToString() + ".");

        var queryResult =
            _queryTranslator.Translate(new LocalQuerying.WebApiQuery(_targetTableauId, routeQuery))
            .Bind(query => _queryManager.RunQuery(query).Result)
            .Map(data => lens.Get(data))
            .Map(data => data.FirstOrDefault());

        return queryResult;
    }

    public Result<IEnumerable<TModel>> GetAll(string? selection = null)
    {
        var lens = new TabularDataObjectLens<TModel>(_targetTableauId.ToString() + ".");

        var queryResult =
            _queryTranslator.Translate(new LocalQuerying.WebApiQuery(_targetTableauId, selection))
            .Bind(query => _queryManager.RunQuery(query).Result)
            .Map(data => lens.Get(data));

        return queryResult;
    }
}

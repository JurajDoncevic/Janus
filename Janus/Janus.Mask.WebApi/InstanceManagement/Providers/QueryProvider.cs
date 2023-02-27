using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Mask.WebApi.InstanceManagement.Templates;
using Janus.Mask.WebApi.LocalQuerying;

namespace Janus.Mask.WebApi.InstanceManagement.Providers;
public class QueryProvider<TId, TModel> where TModel : BaseDto
{
    private readonly WebApiMaskQueryManager _queryManager;

    private readonly TableauId _targetTableauId;
    private readonly AttributeId _identityAttributeId;

    internal QueryProvider(TableauId targetTableauId, AttributeId indetityAttributeId, WebApiMaskQueryManager queryManager)
    {
        _queryManager = queryManager;
        _targetTableauId = targetTableauId;
        _identityAttributeId = indetityAttributeId;
    }


    public Result<TModel> Get(TId id)
    {
        string routeQuery = $"?{_identityAttributeId.AttributeName}={id}"; // leave the translator to do all the work :)

        var queryResult =
            _queryManager.RunQuery<TModel>(new WebApiQuery(_targetTableauId, routeQuery))
            .Result // blocks
            .Map(data => data.Data.FirstOrDefault());

        return queryResult;
    }

    public Result<IEnumerable<TModel>> GetAll(string? selection = null)
    {
        var queryResult =
            _queryManager.RunQuery<TModel>(new WebApiQuery(_targetTableauId, selection))
            .Result // blocks
            .Map(data => Enumerable.AsEnumerable(data.Data));

        return queryResult;
    }
}

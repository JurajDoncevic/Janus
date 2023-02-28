using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Logging;
using Janus.Mask.MaskedDataModel;
using Janus.Mask.WebApi.Lenses;
using Janus.Mask.WebApi.MaskedDataModel;
using Janus.Mask.WebApi.MaskedQueryModel;
using Janus.Mask.WebApi.MaskedSchemaModel;
using Janus.Mask.WebApi.Translation;

namespace Janus.Mask.WebApi;
public sealed class WebApiMaskQueryManager : MaskQueryManager<WebApiQuery, TableauId, string?, Unit, Unit, WebApiTyping>
{
    private readonly WebApiQueryTranslator _queryTranslator;
    private readonly ILogger<WebApiMaskQueryManager>? _logger;
    public WebApiMaskQueryManager(MaskCommunicationNode communicationNode, WebApiMaskSchemaManager schemaManager, WebApiQueryTranslator queryTranslator, ILogger? logger = null) : base(communicationNode, schemaManager, queryTranslator, logger)
    {
        _queryTranslator = queryTranslator;
        logger?.ResolveLogger<WebApiMaskQueryManager>();
    }

    public override async Task<Result<MaskedData<TDto>>> RunQuery<TDto>(WebApiQuery query)
        => await Results.AsResult(() =>
        {
            var lens = new TabularDataObjectLens<TDto>(query.StartingWith.ToString() + ".");

            var queryResult =
                Task.FromResult(_queryTranslator.Translate(query))
                .Bind(query => RunQuery(query))
                .Map(data => lens.Get(data))
                .Map(data => new WebApiDtoData<TDto>(data))
                .Map(data => (MaskedData<TDto>)data); // box it

            return queryResult;
        });

}

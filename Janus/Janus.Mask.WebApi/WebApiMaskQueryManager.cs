using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Logging;
using Janus.Mask.WebApi.MaskedDataModel;
using Janus.Mask.WebApi.MaskedQueryModel;
using Janus.Mask.WebApi.MaskedSchemaModel;
using Janus.Mask.WebApi.Translation;

namespace Janus.Mask.WebApi;
public sealed class WebApiMaskQueryManager : MaskQueryManager<WebApiQuery, TableauId, string?, Unit, Unit, WebApiTyping, WebApiDtoData, object>
{
    private readonly WebApiQueryTranslator _queryTranslator;
    private readonly ILogger<WebApiMaskQueryManager>? _logger;
    public WebApiMaskQueryManager(MaskCommunicationNode communicationNode, WebApiMaskSchemaManager schemaManager, WebApiQueryTranslator queryTranslator, ILogger? logger = null) : base(communicationNode, schemaManager, queryTranslator, logger)
    {
        _queryTranslator = queryTranslator;
        logger?.ResolveLogger<WebApiMaskQueryManager>();
    }

    public override async Task<Result<WebApiDtoData>> RunQuery(WebApiQuery query)
        => await Results.AsResult(() =>
        {
            var dataTranslator = new WebApiDataTranslator(query.StartingWith.ToString() + ".", query.ExpectingReturnDtoType);

            var queryResult =
                Task.FromResult(_queryTranslator.Translate(query))
                .Bind(query => RunQuery(query))
                .Bind(async data => dataTranslator.Translate(data))
                .Map(data => data); // box it

            return queryResult;
        });
}

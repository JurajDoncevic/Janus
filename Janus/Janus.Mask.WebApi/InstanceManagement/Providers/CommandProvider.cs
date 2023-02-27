using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Mask.WebApi.LocalCommanding;

namespace Janus.Mask.WebApi.InstanceManagement.Providers;
public class CommandProvider<TId>
{
    private readonly WebApiMaskCommandManager _commandManager;

    private readonly TableauId _targetTableauId;
    private readonly AttributeId _identityAttributeId;

    internal CommandProvider(TableauId targetTableauId, AttributeId indetityAttributeId, WebApiMaskCommandManager commandManager)
    {
        _commandManager = commandManager;
        _targetTableauId = targetTableauId;
        _identityAttributeId = indetityAttributeId;
    }

    public Result Delete(TId id)
        => Results.AsResult(() =>
        {
            string routeQuery = $"?{_identityAttributeId.AttributeName}={id}"; // leave the translator to do all the work :)
            var result =
                   _commandManager.RunCommand(new WebApiDelete(_targetTableauId, routeQuery))
                   .Result;

            return result;
        });

    public Result Create(object model)
        => Results.AsResult(() =>
        {
            var result =
                   _commandManager.RunCommand(new WebApiInsert(model, _targetTableauId))
                   .Result;

            return result;
        });

    public Result Update(string routeQuery, object model)
        => Results.AsResult(() =>
        {
            var result =
                _commandManager.RunCommand(new WebApiUpdate(_targetTableauId, model, routeQuery))
                .Result;

            return result;
        });
}
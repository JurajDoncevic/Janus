using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Commons.SchemaModels;
using Janus.Mask.WebApi.Translation;

namespace Janus.Mask.WebApi.InstanceManagement.Providers;
public class CommandProvider<TId>
{
    private readonly MaskCommandManager _commandManager;

    private readonly TableauId _targetTableauId;
    private readonly AttributeId _identityAttributeId;

    internal CommandProvider(TableauId targetTableauId, AttributeId indetityAttributeId, MaskCommandManager commandManager)
    {
        _commandManager = commandManager;
        _targetTableauId = targetTableauId;
        _identityAttributeId = indetityAttributeId;
    }

    public Result Delete(TId id)
    {
        string routeQuery = $"?{_identityAttributeId.AttributeName}={id}"; // leave the translator to do all the work :)
        var translatedCommand = CommandTranslation.TranslateDeleteCommand(_targetTableauId, routeQuery);

        var result = _commandManager.RunCommand(translatedCommand).Result;

        return result;
    }

    public Result Create(object model)
    {
        var translatedCommand = CommandTranslation.TranslateInsertCommand(_targetTableauId, model);

        var result = _commandManager.RunCommand(translatedCommand).Result;

        return result;
    }

    public Result Update(string routeQuery, object model)
    {
        var translatedCommand = CommandTranslation.TranslateUpdateCommand(_targetTableauId, model, routeQuery);

        var result = _commandManager.RunCommand(translatedCommand).Result;

        return result;
    }
}

public class FakeCommandManager
{
    public FakeCommandManager() { }

    public Result RunCommand(BaseCommand command)
    {
        return Results.OnSuccess("I'm a big ol' fake! But, lets say everything went OK.");
        //return Results.OnFailure("I'm a big ol' fake! But, lets say everything went Bad.");
    }
}
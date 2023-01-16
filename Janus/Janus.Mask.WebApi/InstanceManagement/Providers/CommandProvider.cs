using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Commons.SchemaModels;
using Janus.Mask.WebApi.Translation;

namespace Janus.Mask.WebApi.InstanceManagement.Providers;
public class CommandProvider<TId>
{
    private readonly FakeCommandManager _commandManager;

    private readonly TableauId _targetTableauId;
    private readonly AttributeId _identityAttributeId;

    public CommandProvider(TableauId targetTableauId, AttributeId indetityAttributeId)
    {
        _commandManager = new FakeCommandManager();
        _targetTableauId = targetTableauId;
        _identityAttributeId = indetityAttributeId;
    }

    public Result Delete(TId id)
    {
        string routeQuery = $"?{_identityAttributeId.AttributeName}={id}"; // leave the translator to do all the work :)
        var translatedCommand = CommandTranslation.TranslateDeleteCommand(_targetTableauId, routeQuery);

        var result = _commandManager.RunCommand(translatedCommand);

        return result;
    }

    public Result Create(object model)
    {
        var translatedCommand = CommandTranslation.TranslateInsertCommand(_targetTableauId, model);

        var result = _commandManager.RunCommand(translatedCommand);

        return result;
    }

    public Result Update(string routeQuery, object model)
    {
        var translatedCommand = CommandTranslation.TranslateUpdateCommand(_targetTableauId, model, routeQuery);

        var result = _commandManager.RunCommand(translatedCommand);

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
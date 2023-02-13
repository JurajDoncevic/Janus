﻿using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Commons.SchemaModels;
using Janus.Mask.WebApi.Translation;

namespace Janus.Mask.WebApi.InstanceManagement.Providers;
public class CommandProvider<TId>
{
    private readonly MaskCommandManager _commandManager;
    private readonly WebApiCommandTranslator _commandTranslator;

    private readonly TableauId _targetTableauId;
    private readonly AttributeId _identityAttributeId;

    internal CommandProvider(TableauId targetTableauId, AttributeId indetityAttributeId, MaskCommandManager commandManager, WebApiCommandTranslator commandTranslator)
    {
        _commandManager = commandManager;
        _targetTableauId = targetTableauId;
        _identityAttributeId = indetityAttributeId;
        _commandTranslator = commandTranslator;
    }

    public Result Delete(TId id)
        => Results.AsResult(() =>
        {
            string routeQuery = $"?{_identityAttributeId.AttributeName}={id}"; // leave the translator to do all the work :)
            var result = 
                _commandTranslator
                    .TranslateDelete(new LocalCommanding.WebApiDelete(_targetTableauId, routeQuery))
                    .Bind(translatedCommand => _commandManager.RunCommand(translatedCommand).Result);

            return result;
        });

    public Result Create(object model)
        => Results.AsResult(() =>
        {
            var result =
                _commandTranslator
                    .TranslateInsert(new LocalCommanding.WebApiInsert(model, _targetTableauId))
                    .Bind(translatedCommand => _commandManager.RunCommand(translatedCommand).Result);

            return result;
        });

    public Result Update(string routeQuery, object model)
        => Results.AsResult(() =>
        {
            var result =
                _commandTranslator
                    .TranslateUpdate(new LocalCommanding.WebApiUpdate(_targetTableauId, model, routeQuery))
                    .Bind(translatedCommand => _commandManager.RunCommand(translatedCommand).Result);

            return result;
        });
}
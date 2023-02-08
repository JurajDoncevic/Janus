using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Components;
using Janus.Logging;
using Janus.Wrapper.LocalCommanding;
using Janus.Wrapper.Translation;

namespace Janus.Wrapper;
/// <summary>
/// Command manager for a wrapper component
/// </summary>
/// <typeparam name="TDeleteCommand"></typeparam>
/// <typeparam name="TInsertCommand"></typeparam>
/// <typeparam name="TUpdateCommand"></typeparam>
/// <typeparam name="TSelection"></typeparam>
/// <typeparam name="TMutation"></typeparam>
/// <typeparam name="TInstantiation"></typeparam>
public abstract class WrapperCommandManager<TDeleteCommand, TInsertCommand, TUpdateCommand, TSelection, TMutation, TInstantiation>
    : IComponentCommandManager
    where TDeleteCommand : LocalDelete<TSelection>
    where TInsertCommand : LocalInsert<TInstantiation>
    where TUpdateCommand : LocalUpdate<TSelection, TMutation>
{
    private readonly ILocalCommandTranslator<TDeleteCommand, TInsertCommand, TUpdateCommand, TSelection, TMutation, TInstantiation> _commandTranslator;
    private readonly ICommandExecutor<TDeleteCommand, TInsertCommand, TUpdateCommand, TSelection, TMutation, TInstantiation> _commandExecutor;
    private readonly ILogger<WrapperCommandManager<TDeleteCommand, TInsertCommand, TUpdateCommand, TSelection, TMutation, TInstantiation>>? _logger;

    public WrapperCommandManager(
        ILocalCommandTranslator<TDeleteCommand, TInsertCommand, TUpdateCommand, TSelection, TMutation, TInstantiation> commandTranslator,
        ICommandExecutor<TDeleteCommand, TInsertCommand, TUpdateCommand, TSelection, TMutation, TInstantiation> commandExecutor,
        ILogger? logger = null )
    {
        _commandTranslator = commandTranslator;
        _commandExecutor = commandExecutor;
        _logger = logger?.ResolveLogger<WrapperCommandManager<TDeleteCommand, TInsertCommand, TUpdateCommand, TSelection, TMutation, TInstantiation>>();
    }

    public async Task<Result> RunCommand(BaseCommand command)
        => await (command switch
        {
            DeleteCommand deleteCommand => Task.FromResult(_commandTranslator.TranslateDelete(deleteCommand))
                                            .Bind(_commandExecutor.ExecuteDeleteCommand),
            InsertCommand insertCommand => Task.FromResult(_commandTranslator.TranslateInsert(insertCommand))
                                            .Bind(_commandExecutor.ExecuteInsertCommand),
            UpdateCommand updateCommand => Task.FromResult(_commandTranslator.TranslateUpdate(updateCommand))
                                            .Bind(_commandExecutor.ExecuteUpdateCommand),
            _ => Task.FromResult(Results.OnFailure("Unknown command type"))
        });

}

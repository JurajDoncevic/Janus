using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;
using Janus.Components;
using Janus.Wrapper.LocalCommanding;
using Janus.Wrapper.Translation;

namespace Janus.Wrapper;
public sealed class WrapperCommandManager<TSelection, TMutation, TInstantiation> : IComponentCommandManager
{
    private readonly ILocalCommandTranslator<LocalCommand, TSelection, TMutation, TInstantiation> _commandTranslator;
    private readonly ICommandExecutor<TSelection, TMutation, TInstantiation> _commandExecutor;

    public WrapperCommandManager(
        ILocalCommandTranslator<LocalCommand, TSelection, TMutation, TInstantiation> commandTranslator,
        ICommandExecutor<TSelection, TMutation, TInstantiation> commandExecutor)
    {
        _commandTranslator = commandTranslator;
        _commandExecutor = commandExecutor;
    }

    public async Task<Result> RunCommand(BaseCommand command)
        => await Task.FromResult(_commandTranslator.Translate(command))
            .Bind(_commandExecutor.ExecuteCommand);

}

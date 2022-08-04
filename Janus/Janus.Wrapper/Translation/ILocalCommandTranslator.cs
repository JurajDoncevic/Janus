using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;
using Janus.Components.Translation;
using Janus.Wrapper.LocalCommanding;

namespace Janus.Wrapper.Translation;
public interface ILocalCommandTranslator<TLocalCommand, TSelection, TMutation, TInstantiation>
    : ICommandTranslator<BaseCommand, CommandSelection, Mutation, Instantiation, TLocalCommand, TSelection, TMutation, TInstantiation>
    where TLocalCommand : LocalCommand
{
}

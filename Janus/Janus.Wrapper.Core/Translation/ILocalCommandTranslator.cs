using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;
using Janus.Components.Translation;
using Janus.Wrapper.Core.LocalCommanding;

namespace Janus.Wrapper.Core.Translation;
public interface ILocalCommandTranslator<TLocalCommand, TSelection, TMutation, TInstantiation>
    : ICommandTranslator<BaseCommand, CommandSelection, Mutation, Instantiation, TLocalCommand, TSelection, TMutation, TInstantiation>
    where TLocalCommand : LocalCommand
{
}

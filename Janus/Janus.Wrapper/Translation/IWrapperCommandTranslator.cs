using Janus.Commons.CommandModels;
using Janus.Components.Translation;
using Janus.Wrapper.LocalCommanding;

namespace Janus.Wrapper.Translation;

public interface IWrapperCommandTranslator<TDeleteCommand, TInsertCommand, TUpdateCommand, TSelection, TMutation, TInstantiation>
    : ICommandTranslator<DeleteCommand, InsertCommand, UpdateCommand, CommandSelection, Mutation, Instantiation,
                         TDeleteCommand, TInsertCommand, TUpdateCommand, TSelection, TMutation, TInstantiation>
    where TDeleteCommand : LocalDelete<TSelection>
    where TInsertCommand : LocalInsert<TInstantiation>
    where TUpdateCommand : LocalUpdate<TSelection, TMutation>
{

}

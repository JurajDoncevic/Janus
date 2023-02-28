using Janus.Commons.CommandModels;
using Janus.Commons.SelectionExpressions;
using Janus.Components.Translation;
using Janus.Mask.MaskedCommandModel;

namespace Janus.Mask.Translation;
public interface IMaskCommandTranslator<TMaskedDeleteCommand, TMaskedInsertCommand, TMaskedUpdateCommand, TMaskedSelection, TMaskedMutation, TMaskedInstantiation>
    : ICommandTranslator<TMaskedDeleteCommand, TMaskedInsertCommand, TMaskedUpdateCommand, TMaskedSelection, TMaskedMutation, TMaskedInstantiation,
                         DeleteCommand, InsertCommand, UpdateCommand, SelectionExpression, Mutation, Instantiation>
    where TMaskedDeleteCommand : MaskedDelete<TMaskedSelection>
    where TMaskedInsertCommand : MaskedInsert<TMaskedInstantiation>
    where TMaskedUpdateCommand : MaskedUpdate<TMaskedSelection, TMaskedMutation>
{

}
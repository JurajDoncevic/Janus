using Janus.Commons.CommandModels;
using Janus.Commons.SelectionExpressions;
using Janus.Components.Translation;
using Janus.Mask.LocalCommanding;

namespace Janus.Mask.Translation;
public interface IMaskCommandTranslator<TDeleteCommand, TInsertCommand, TUpdateCommand, TSelection, TMutation, TInstantiation>
    : ICommandTranslator<TDeleteCommand, TInsertCommand, TUpdateCommand, TSelection, TMutation, TInstantiation,
                         DeleteCommand, InsertCommand, UpdateCommand, SelectionExpression, Mutation, Instantiation>
    where TDeleteCommand : LocalDelete<TSelection>
    where TInsertCommand : LocalInsert<TInstantiation>
    where TUpdateCommand : LocalUpdate<TSelection, TMutation>
{

}
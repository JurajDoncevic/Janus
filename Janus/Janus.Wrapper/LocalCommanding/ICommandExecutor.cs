﻿using FunctionalExtensions.Base.Results;

namespace Janus.Wrapper.LocalCommanding;

public interface ICommandExecutor<TDeleteCommand, TInsertCommand, TUpdateCommand, TSelection, TMutation, TInstantiation>
    where TDeleteCommand : LocalDelete<TSelection>
    where TInsertCommand : LocalInsert<TInstantiation>
    where TUpdateCommand : LocalUpdate<TSelection, TMutation>
{
    public Task<Result> ExecuteDeleteCommand(TDeleteCommand command);
    public Task<Result> ExecuteInsertCommand(TInsertCommand command);
    public Task<Result> ExecuteUpdateCommand(TUpdateCommand command);
}
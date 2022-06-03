using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;


public interface IPostInitDeleteCommandBuilder
{
    IPostInitDeleteCommandBuilder WithSelection(Func<CommandSelectionBuilder, CommandSelectionBuilder> configuration);
    DeleteCommand Build();
}


public class DeleteCommandBuilder : IPostInitDeleteCommandBuilder
{
    private readonly string _onTableauId;
    private readonly DataSource _dataSource;
    private Option<CommandSelection> _selection;

    internal DeleteCommandBuilder(string onTableauId!!, DataSource dataSource!!)
    {
        _onTableauId = onTableauId;
        _dataSource = dataSource;
        _selection = Option<CommandSelection>.None;
    }

    public static IPostInitDeleteCommandBuilder InitOnDataSource(string onTableauId!!, DataSource dataSource!!)
    {
        if (!dataSource.ContainsTableau(onTableauId))
            throw new TableauDoesNotExistException(onTableauId, dataSource.Name);

        return new DeleteCommandBuilder(onTableauId, dataSource);
    }

    public IPostInitDeleteCommandBuilder WithSelection(Func<CommandSelectionBuilder, CommandSelectionBuilder> configuration)
    {
        var selectionBuilder = new CommandSelectionBuilder(_dataSource, new() { _onTableauId });
        _selection = Option<CommandSelection>.Some(configuration(selectionBuilder).Build());

        return this;
    }

    public DeleteCommand Build()
    {
        return new DeleteCommand(_onTableauId, _selection);
    }
}

using Janus.Commons.SelectionExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;

public class UpdateCommand
{
    private readonly string _onTableauId;
    private readonly Mutation _mutation;
    private readonly SelectionExpression _selection;

    public UpdateCommand(string onTableauId!!, Mutation mutation!!, SelectionExpression selection!!)
    {
        _onTableauId = onTableauId;
        _mutation = mutation;
        _selection = selection;
    }

    public string OnTableauId => _onTableauId;

    public Mutation Mutation => _mutation;

    public SelectionExpression Selection => _selection;
}

using Janus.Commons.CommandModels.Exceptions;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;


public class InsertCommandOpenBuilder
{
    private readonly string _onTableauId;
    private Option<Instantiation> _instantiation;
    internal InsertCommandOpenBuilder(string onTableauId)
    {
        _onTableauId = onTableauId;
        _instantiation = Option<Instantiation>.None;
    }

    public InsertCommand Build()
        => _instantiation
           ? new InsertCommand(_onTableauId, _instantiation.Value)
           : throw new InstantiationNotSetException();

    public static InsertCommandOpenBuilder InitOpenInsert(string onTableauId)
    {
        return new InsertCommandOpenBuilder(onTableauId);
    }

    public InsertCommandOpenBuilder WithInstantiation(Func<InstantiationOpenBuilder, InstantiationOpenBuilder> configuration)
    {
        var instantiationBuilder = new InstantiationOpenBuilder(_onTableauId);
        _instantiation = Option<Instantiation>.Some(configuration(instantiationBuilder).Build());

        return this;
    }
}

public class InstantiationOpenBuilder
{
    private readonly string _onTableauId;
    private TabularData? _tableauDataToInsert;

    internal InstantiationOpenBuilder(string onTableauId)
    {
        _onTableauId = onTableauId;
        _tableauDataToInsert = null;
    }

    public InstantiationOpenBuilder WithValues(TabularData tableauData)
    {
        _tableauDataToInsert = tableauData;
        return this;
    }

    internal Instantiation Build()
    {
        return _tableauDataToInsert == null
                ? throw new InstantiationNotSetException()
                : new Instantiation(_tableauDataToInsert);
    }
}

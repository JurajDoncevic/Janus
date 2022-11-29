using Janus.Commons.CommandModels.Exceptions;
using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;

namespace Janus.Commons.CommandModels;

/// <summary>
/// Builder class to internally construct an insert command without validation on a data source
/// </summary>
public sealed class InsertCommandOpenBuilder
{
    private readonly TableauId _onTableauId;
    private Option<Instantiation> _instantiation;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    private InsertCommandOpenBuilder(TableauId onTableauId)
    {
        _onTableauId = onTableauId;
        _instantiation = Option<Instantiation>.None;
    }

    /// <summary>
    /// Builds the specified insert command
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InstantiationNotSetException"></exception>
    public InsertCommand Build()
        => _instantiation
           ? new InsertCommand(_onTableauId, _instantiation.Value)
           : throw new InstantiationNotSetException();

    /// <summary>
    /// Initializes the open builder on the starting tableau
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <returns></returns>
    public static InsertCommandOpenBuilder InitOpenInsert(TableauId onTableauId)
    {
        return new InsertCommandOpenBuilder(onTableauId);
    }

    /// <summary>
    /// Initializes the open builder on the starting tableau
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <returns></returns>
    public static InsertCommandOpenBuilder InitOpenInsert(string onTableauId)
        => InitOpenInsert(TableauId.From(onTableauId));

    /// <summary>
    /// Sets the instantiation clause
    /// </summary>
    /// <param name="configuration">Instantiation configuration</param>
    /// <returns></returns>
    public InsertCommandOpenBuilder WithInstantiation(Func<InstantiationOpenBuilder, InstantiationOpenBuilder> configuration)
    {
        var instantiationBuilder = new InstantiationOpenBuilder(_onTableauId);
        _instantiation = Option<Instantiation>.Some(configuration(instantiationBuilder).Build());

        return this;
    }
}

/// <summary>
/// Open builder for the instatiation clause
/// </summary>
public sealed class InstantiationOpenBuilder
{
    private readonly TableauId _onTableauId;
    private TabularData? _tabularDataToInsert;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    internal InstantiationOpenBuilder(TableauId onTableauId)
    {
        _onTableauId = onTableauId;
        _tabularDataToInsert = null;
    }

    /// <summary>
    /// Sets the instantiation values
    /// </summary>
    /// <param name="tabularData">Data to be inserted. <b>The attributes are qualified by their names.</b> Don't use attribute ids</param>
    /// <returns></returns>
    public InstantiationOpenBuilder WithValues(TabularData tabulardata)
    {
        _tabularDataToInsert = tabulardata;
        return this;
    }

    /// <summary>
    /// Builds the specified instantiation clause
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InstantiationNotSetException"></exception>
    internal Instantiation Build()
    {
        return _tabularDataToInsert == null
                ? throw new InstantiationNotSetException()
                : new Instantiation(_tabularDataToInsert);
    }
}

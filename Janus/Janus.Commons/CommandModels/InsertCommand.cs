using Janus.Commons.CommandModels.Exceptions;
using Janus.Commons.CommandModels.JsonConversion;
using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;

[JsonConverter(typeof(InsertCommandJsonConverter))]
public class InsertCommand : BaseCommand
{
    private readonly Instantiation _instantiation;

    /// <summary>
    /// Instantiation clause
    /// </summary>
    public Instantiation Instantiation => _instantiation;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="instantiation">Instantiation clause</param>
    internal InsertCommand(string onTableauId!!, Instantiation instantiation!!) : base(onTableauId)
    {
        _instantiation = instantiation;
    }

    public override Result IsValidForDataSource(DataSource dataSource)
        => ResultExtensions.AsResult(() =>
        {
            InsertCommandBuilder.InitOnDataSource(_onTableauId, dataSource)
                .WithInstantiation(configuration => configuration.WithValues(_instantiation.TabularData))
                .Build();
 
            return true;
        });

    public override bool Equals(object? obj)
    {
        return obj is InsertCommand command &&
               _onTableauId.Equals(command._onTableauId) &&
               _instantiation.Equals(command._instantiation);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_onTableauId, _instantiation);
    }
}

using Janus.Commons.SchemaModels.Exceptions;

namespace Janus.Commons.SchemaModels.Building;

/// <summary>
/// Builder for a tableau
/// </summary>
public class TableauBuilder : ITableauBuilder
{
    private Tableau? _tableau;
    private string _tableauName;
    private Schema _parentSchema;
    private string _tableauDescription;
    private HashSet<UpdateSet> _updateSets;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tableauName">Tableau name</param>
    /// <param name="parentSchema">Schema inside which the builder works</param>
    /// <param name="tableauDescription">Tableau's description</param>
    internal TableauBuilder(string tableauName, Schema parentSchema, string? tableauDescription = "", IEnumerable<UpdateSet>? updateSets = null)
    {
        _tableau = null;
        _tableauName = tableauName ?? throw new ArgumentNullException($"'{nameof(tableauName)}' cannot be null or empty.", nameof(tableauName));
        _tableauDescription = tableauDescription ?? string.Empty;
        _parentSchema = parentSchema ?? throw new ArgumentNullException(nameof(parentSchema));
        _updateSets = new HashSet<UpdateSet>(updateSets ?? Enumerable.Empty<UpdateSet>());
    }

    /// <summary>
    /// Adds an attribute with given name and configuration
    /// </summary>
    /// <param name="attributeName">Attribute name</param>
    /// <param name="configuration">Build configuration</param>
    /// <returns></returns>
    /// <exception cref="AttributeOrdinalAssignedException"></exception>
    /// <exception cref="AttributeNameAssignedException"></exception>
    public IAttributeAdding AddAttribute(string attributeName, Func<IAttributeBuilder, IAttributeBuilder> configuration)
    {
        if (_tableau is null)
        {
            _tableau = new Tableau(_tableauName, _parentSchema, _tableauDescription);
        }

        var attribute = configuration(new AttributeBuilder(attributeName, _tableau)).Build();

        if (_tableau.Attributes.ToList().Exists(a => a.Ordinal == attribute.Ordinal))
            throw new AttributeOrdinalAssignedException(attribute.Ordinal, attributeName, _tableau.Name);
        if (_tableau.AttributeNames.Contains(attributeName))
            throw new AttributeNameAssignedException(attributeName, _tableau.Name);

        _tableau.AddAttribute(attribute);
        return this;
    }

    public ITableauEditing WithDescription(string description)
    {
        _tableauDescription = description ?? _tableauDescription;
        return this;
    }

    public ITableauEditing WithName(string name)
    {
        _tableauName = name ?? _tableauName;
        return this;
    }

    /// <summary>
    /// Builds the tableau
    /// </summary>
    /// <returns></returns>
    public Tableau Build()
    {
        return _tableau ?? new Tableau(_tableauName, _parentSchema, _tableauDescription);
    }

    public IUpdateSetAdding AddUpdateSet(UpdateSet updateSet)
    {
        if (_tableau is null)
        {
            _tableau = new Tableau(_tableauName, _parentSchema, _tableauDescription);
        }
        _tableau.AddUpdateSet(updateSet);
        return this;
    }

    public IUpdateSetAdding AddUpdateSet(Func<IUpdateSetBuilder, IUpdateSetBuilder> configuration)
    {
        if (_tableau is null)
        {
            _tableau = new Tableau(_tableauName, _parentSchema, _tableauDescription);
        }
        var updateSet = configuration(new UpdateSetBuilder(_tableau)).Build();

        _updateSets.Add(updateSet);

        // check if update sets overlap
        bool existsOverlap =
            _updateSets.SelectMany(us2 => _updateSets.Map(us1 => (us1, us2)))
                  .Where(tuple => !tuple.us1.Equals(tuple.us2))
                  .Any(tuple => tuple.us1.OverlapsWith(tuple.us2));

        if (existsOverlap)
        {
            throw new UpdateSetsOverlapException(_updateSets, _tableauName);
        }

        return this;
    }

    public IUpdateSetAdding WithUpdateSets(HashSet<UpdateSet> updateSets)
    {
        bool existsUnknownAttributeId =
            updateSets.SelectMany(us => us.AttributeIds)
            .All(attrId => (_tableau?.Attributes.Select(a => a.Id) ?? Enumerable.Empty<string>()).Contains(attrId));

        // check if update sets overlap
        bool existsOverlap =
            updateSets.SelectMany(us2 => updateSets.Map(us1 => (us1, us2)))
                  .Where(tuple => !tuple.us1.Equals(tuple.us2))
                  .Any(tuple => tuple.us1.OverlapsWith(tuple.us2));

        if (existsOverlap)
        {
            throw new UpdateSetsOverlapException(updateSets, _tableauName);
        }

        if (_tableau is null)
        {
            _tableau = new Tableau(_tableauName, _parentSchema, _tableauDescription);
        }
        // clear update sets
        _tableau =
            _tableau.UpdateSets.Fold(_tableau, (updateSet, tableau) =>
            {
                tableau.RemoveUpdateSet(updateSet);
                return tableau;
            });
        // add new update sets
        _tableau =
            updateSets.Fold(_tableau, (updateSet, tableau) =>
            {
                if (!updateSet.IsEmpty())
                {
                    tableau.AddUpdateSet(updateSet);
                }
                return tableau;
            });
        return this;
    }

    public IUpdateSetAdding WithDefaultUpdateSet()
    {
        if (_tableau is null)
        {
            _tableau = new Tableau(_tableauName, _parentSchema, _tableauDescription);
        }
        // clear update sets
        _tableau =
            _tableau.UpdateSets.Fold(_tableau, (updateSet, tableau) =>
            {
                tableau.RemoveUpdateSet(updateSet);
                return tableau;
            });
        // add new update set
        UpdateSet updateSet = new UpdateSet(_tableau.Attributes.Select(attr => attr.Id).ToHashSet());

        _tableau.AddUpdateSet(updateSet);

        return this;
    }
}


public interface ITableauBuilding
{
    /// <summary>
    /// Builds the tableau
    /// </summary>
    /// <returns></returns>
    Tableau Build();
}

public interface ITableauEditing : IAttributeAdding
{
    /// <summary>
    /// Sets the tableau name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    ITableauEditing WithName(string name);
    /// <summary>
    /// Sets the tableau description
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    ITableauEditing WithDescription(string description);
}

public interface IAttributeAdding : IUpdateSetAdding
{
    /// <summary>
    /// Adds an attribute with given name and configuration
    /// </summary>
    /// <param name="attributeName">Attribute name</param>
    /// <param name="configuration">Build configuration</param>
    /// <returns></returns>
    /// <exception cref="AttributeOrdinalAssignedException"></exception>
    /// <exception cref="AttributeNameAssignedException"></exception>
    IAttributeAdding AddAttribute(string attributeName, Func<IAttributeBuilder, IAttributeBuilder> configuration);
}

public interface IUpdateSetAdding : ITableauBuilding
{
    /// <summary>
    /// Adds an update set to the tableau
    /// </summary>
    /// <param name="updateSet"></param>
    /// <returns></returns>
    IUpdateSetAdding AddUpdateSet(UpdateSet updateSet);

    /// <summary>
    /// Adds update sets to the tableau. Clears previously added update sets
    /// </summary>
    /// <param name="updateSet"></param>
    /// <returns></returns>
    IUpdateSetAdding WithUpdateSets(HashSet<UpdateSet> updateSets);

    /// <summary>
    /// Adds update set to tableau that covers all atributes. 
    /// </summary>
    /// <param name="updateSets"></param>
    /// <returns></returns>
    IUpdateSetAdding WithDefaultUpdateSet();

    /// <summary>
    /// Adds update set to tableau according to the configuration
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public IUpdateSetAdding AddUpdateSet(Func<IUpdateSetBuilder, IUpdateSetBuilder> configuration);
}

public interface ITableauBuilder : ITableauEditing
{

}
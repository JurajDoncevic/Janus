namespace Janus.Commons.SchemaModels;

/// <summary>
/// Describes a schema
/// </summary>
public sealed class Schema
{
    private readonly string _name;
    private readonly string _description;
    private readonly DataSource _dataSource;
    private readonly Dictionary<string, Tableau> _tableaus;

    /// <summary>
    /// Schema ID
    /// </summary>
    public string Id => _dataSource.Id + "." + _name;
    /// <summary>
    /// Schema name
    /// </summary>
    public string Name => _name;
    /// <summary>
    /// Schema description
    /// </summary>
    public string Description => _description;
    /// <summary>
    /// Tableaus in the schema
    /// </summary>
    public ReadOnlyCollection<Tableau> Tableaus => _tableaus.Values.ToList().AsReadOnly();
    /// <summary>
    /// Parent data source
    /// </summary>
    public DataSource DataSource => _dataSource;
    /// <summary>
    /// Names of tableaus in this schema
    /// </summary>
    public List<string> TableauNames => _tableaus.Keys.ToList();
    /// <summary>
    /// Get tableau with name
    /// </summary>
    /// <param name="tableauName"></param>
    /// <returns></returns>
    public Tableau this[string tableauName] => _tableaus[tableauName];

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Schema name</param>
    /// <param name="dataSource">Parent data source</param>
    internal Schema(string name, DataSource dataSource, string description = "")
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        }

        _name = name;
        _description = description;
        _tableaus = new();
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Schema name</param>
    /// <param name="dataSource">Parent data source</param>
    /// <param name="tableaus">Underlying tableaus</param>
    internal Schema(string name, List<Tableau> tableaus, DataSource dataSource, string description = "")
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        }

        if (tableaus is null)
        {
            throw new ArgumentNullException(nameof(tableaus));
        }

        _name = name;
        _description = description;
        _tableaus = tableaus.ToDictionary(tableau => tableau.Name, tableau => tableau);
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
    }

    /// <summary>
    /// Adds a new unique tableau to the schema
    /// </summary>
    /// <param name="tableau">Tableau object</param>
    /// <returns>true if new tableau is added, false if a tableau with the given name exists</returns>
    internal bool AddTableau(Tableau tableau)
    {
        if (tableau is null)
        {
            throw new ArgumentNullException(nameof(tableau));
        }

        if (!_tableaus.ContainsKey(tableau.Name))
        {
            tableau = new Tableau(tableau.Name, tableau.Attributes.ToList(), this); // copy just in case to assign correct parent
            _tableaus.Add(tableau.Name, tableau);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes a tableau with the given name
    /// </summary>
    /// <param name="tableauName">Tableau name</param>
    /// <returns>true if a tableau is found and removed, else false</returns>
    internal bool RemoveTableau(string tableauName)
    {
        if (tableauName is null)
        {
            throw new ArgumentNullException(nameof(tableauName));
        }

        return _tableaus.Remove(tableauName);
    }

    public override bool Equals(object? obj)
    {
        return obj is Schema schema &&
               _name.Equals(schema._name) &&
               _dataSource.Id.Equals(schema._dataSource.Id) &&
               _description.Equals(schema._description) &&
               _tableaus.SequenceEqual(schema._tableaus);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_name, _dataSource, _tableaus);
    }

    public static bool operator ==(Schema? left, Schema? right)
    {
        return EqualityComparer<Schema>.Default.Equals(left, right);
    }

    public static bool operator !=(Schema? left, Schema? right)
    {
        return !(left == right);
    }

    public override string ToString()
        => $"({Name} \n({string.Join("\n", Tableaus)}))";
}

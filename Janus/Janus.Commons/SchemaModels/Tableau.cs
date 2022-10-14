namespace Janus.Commons.SchemaModels;

/// <summary>
/// Describes a tableau
/// </summary>
public sealed class Tableau
{
    private readonly string _name;
    private readonly Schema _schema;
    private readonly string _description;
    private readonly Dictionary<string, Attribute> _attributes;
    private readonly HashSet<UpdateSet> _updateSets;

    /// <summary>
    /// Tableau ID
    /// </summary>
    public string Id => _schema.Id + "." + _name;
    /// <summary>
    /// Tableau name
    /// </summary>
    public string Name => _name;
    /// <summary>
    /// Tableau description
    /// </summary>
    public string Description => _description;
    /// <summary>
    /// Parent schema
    /// </summary>
    public Schema Schema => _schema;
    /// <summary>
    /// Attributes in the tableau
    /// </summary>
    public ReadOnlyCollection<Attribute> Attributes => _attributes.Values.ToList().AsReadOnly();
    /// <summary>
    /// Names of attributes in this tableau
    /// </summary>
    public List<string> AttributeNames => _attributes.Keys.ToList();
    /// <summary>
    /// Attribute update sets
    /// </summary>
    public ReadOnlyCollection<UpdateSet> UpdateSets => _updateSets.ToList().AsReadOnly();

    /// <summary>
    /// Get attribute with name
    /// </summary>
    /// <param name="attributeName"></param>
    /// <returns></returns>
    public Attribute this[string attributeName] => _attributes[attributeName];

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Tableau name</param>
    /// <param name="schema">Parent schema</param>
    /// <param name="attributes">Underlying attributes</param>
    internal Tableau(string name, List<Attribute> attributes, Schema schema, HashSet<UpdateSet>? updateSets = null, string description = "")
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        }

        if (attributes is null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        _name = name;
        _description = description;
        _schema = schema ?? throw new ArgumentNullException(nameof(schema));
        _attributes = attributes.ToDictionary(attribute => attribute.Name, attribute => attribute);
        _updateSets = updateSets ?? new HashSet<UpdateSet>();
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Tableau name</param>
    /// <param name="schema">Parent schema</param>
    internal Tableau(string name, Schema schema, string description = "")
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        }

        _name = name;
        _description = description;
        _schema = schema ?? throw new ArgumentNullException(nameof(schema));
        _attributes = new();
        _updateSets = new();
    }

    /// <summary>
    /// Adds a new unique attribute to the tableau
    /// </summary>
    /// <param name="attribute">Attribute object</param>
    /// <returns>true if new attribute is added, false if an attribute with the given name or ordinal exists</returns>
    internal bool AddAttribute(Attribute attribute)
    {
        if (attribute is null)
        {
            throw new ArgumentNullException(nameof(attribute));
        }

        int automaticOrdinal = attribute.Ordinal < 0
            ? (_attributes.Values.Max(a => a.Ordinal as int?) ?? -1) + 1
            : attribute.Ordinal;


        if (!_attributes.ContainsKey(attribute.Name) && !_attributes.Values.ToList().Exists(a => a.Ordinal == attribute.Ordinal))
        {
            attribute = new Attribute(attribute.Name, attribute.DataType, attribute.IsIdentity, attribute.IsNullable, automaticOrdinal, this);
            _attributes.Add(attribute.Name, attribute);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Removes an attribute with given name
    /// </summary>
    /// <param name="attributeName">Attribute name</param>
    /// <returns>true if a attribute is found and removed, else false</returns>
    internal bool RemoveAttribute(string attributeName)
    {
        if (attributeName is null)
        {
            throw new ArgumentNullException(nameof(attributeName));
        }

        return _attributes.Remove(attributeName);
    }

    /// <summary>
    /// Add an update set on the tableau
    /// </summary>
    /// <param name="updateSet"></param>
    /// <returns>True on success, false on failure or if an overlap would be created</returns>
    internal bool AddUpdateSet(UpdateSet updateSet)
    {
        if(updateSet.AttributeNames.All(attrId => Attributes.Select(attr => attr.Id).Contains(attrId)) && // all attributes in update set exist on this tableau
           !_updateSets.Any(us => us.OverlapsWith(updateSet))) // no current update sets overlap with the new update set
        {
            return _updateSets.Add(updateSet);
        }
        // overlap found
        return false;
    }

    /// <summary>
    /// Remove an update set from the tablueau
    /// </summary>
    /// <param name="updateSet"></param>
    /// <returns></returns>
    internal bool RemoveUpdateSet(UpdateSet updateSet)
    {
        return _updateSets.Remove(updateSet);
    }

    public override bool Equals(object? obj)
    {
        return obj is Tableau tableau &&
               _name.Equals(tableau._name) &&
               _attributes.SequenceEqual(tableau._attributes) &&
               _schema.Id.Equals(tableau.Schema.Id) &&
               _description.Equals(tableau._description) &&
               _updateSets.SetEquals(tableau._updateSets) &&
               Id.Equals(Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_name, _schema, _attributes, Id);
    }

    public override string ToString()
        => $"({Name} \n({string.Join("\n", Attributes)}))";
}

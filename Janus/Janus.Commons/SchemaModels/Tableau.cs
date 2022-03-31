
namespace Janus.Commons.SchemaModels;

/// <summary>
/// Describes a tableau
/// </summary>
public class Tableau
{
    private readonly string _name;
    private readonly Schema _schema;
    private readonly Dictionary<string, Attribute> _attributes;

    /// <summary>
    /// Tableau ID
    /// </summary>
    public string Id => _schema.Id + "." + _name;
    /// <summary>
    /// Tableau name
    /// </summary>
    public string Name => _name;
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
    /// Constructor
    /// </summary>
    /// <param name="name">Tableau name</param>
    /// <param name="schema">Parent schema</param>
    /// <param name="attributes">Underlying attributes</param>
    internal Tableau(string name!!, Schema schema!!, List<Attribute> attributes!!)
    {
        _name = name;
        _schema = schema;
        _attributes = attributes.ToDictionary(attribute => attribute.Name, attribute => attribute); ;
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Tableau name</param>
    /// <param name="schema">Parent schema</param>
    internal Tableau(string name!!, Schema schema!!)
    {
        _name = name;
        _schema = schema;
        _attributes = new();
    }

    /// <summary>
    /// Adds a new unique attribute to the tableau
    /// </summary>
    /// <param name="attribute">Attribute object</param>
    /// <returns>true if new attribute is added, false if an attribute with the given name exists</returns>
    internal bool AddAttribute(Attribute attribute!!)
    {
        if (!_attributes.ContainsKey(attribute.Name))
        {
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
    internal bool RemoveAttribute(string attributeName!!)
    {
        return _attributes.Remove(attributeName);
    }

    public override bool Equals(object? obj)
    {
        return obj is Tableau tableau &&
               _name == tableau._name &&
               EqualityComparer<Schema>.Default.Equals(_schema, tableau._schema) &&
               EqualityComparer<Dictionary<string, Attribute>>.Default.Equals(_attributes, tableau._attributes) &&
               Id == tableau.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_name, _schema, _attributes, Id);
    }
}

namespace Janus.Commons.SchemaModels;

/// <summary>
/// Describes an attribute
/// </summary>
public sealed class Attribute
{
    private readonly string _name;
    private readonly DataTypes _dataType;
    private readonly bool _isIdentity;
    private readonly bool _isNullable;
    private readonly int _ordinal;
    private readonly string _description;
    private readonly Tableau _tableau;

    /// <summary>
    /// Attribute ID
    /// </summary>
    public string Id => _tableau.Id + "." + _name;
    /// <summary>
    /// Attribute name
    /// </summary>
    public string Name => _name;
    /// <summary>
    /// Attribute data type
    /// </summary>
    public DataTypes DataType => _dataType;
    /// <summary>
    /// Is this attribute an identity or part of a composite identity
    /// </summary>
    public bool IsIdentity => _isIdentity;
    /// <summary>
    /// Is this attribute's value nullable
    /// </summary>
    public bool IsNullable => _isNullable;
    /// <summary>
    /// Ordinal number position of the attribute
    /// </summary>
    public int Ordinal => _ordinal;
    /// <summary>
    /// Attribute description
    /// </summary>
    public string Description => _description;
    /// <summary>
    /// Parent tableau
    /// </summary>
    public Tableau Tableau => _tableau;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Attribute name</param>
    /// <param name="dataType">Attribute data type</param>
    /// <param name="isIdentity">Is this attribute part a primary key or part of a composite key</param>
    /// <param name="isNullable">Is this attribute's value nullable</param>
    /// <param name="ordinal">Ordinal number position of the attribute</param>
    /// <param name="description">Attribute description</param>
    /// <param name="tableau">Parent tableau</param>
    internal Attribute(string name, DataTypes dataType, bool isIdentity, bool isNullable, int ordinal, Tableau tableau, string description = "")
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        }

        _name = name;
        _dataType = dataType;
        _isIdentity = isIdentity;
        _isNullable = isNullable;
        _ordinal = ordinal;
        _description = description;
        _tableau = tableau ?? throw new ArgumentNullException(nameof(tableau));
    }

    public override bool Equals(object? obj)
    {
        return obj is Attribute attribute &&
               _name == attribute._name &&
               _dataType == attribute._dataType &&
               _isIdentity == attribute._isIdentity &&
               _isNullable == attribute._isNullable &&
               _ordinal == attribute._ordinal &&
               _tableau.Id == attribute._tableau.Id &&
               _description.Equals(attribute._description);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_name, _dataType, _isIdentity, _isNullable, _ordinal, _tableau, _description);
    }

    public static bool operator ==(Attribute? left, Attribute? right)
    {
        return EqualityComparer<Attribute>.Default.Equals(left, right);
    }

    public static bool operator !=(Attribute? left, Attribute? right)
    {
        return !(left == right);
    }

    public override string ToString()
        => $"({Name} {DataType} {Ordinal} {IsNullable} {IsIdentity})";
}

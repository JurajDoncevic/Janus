namespace Janus.Commons.SchemaModels;

/// <summary>
/// Describes an attribute
/// </summary>
public class Attribute
{
    private readonly string _name;
    private readonly DataTypes _dataType;
    private readonly bool _isPrimaryKey;
    private readonly bool _isNullable;
    private readonly int _ordinal;
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
    /// Is this attribute a primary key or part of a composite key
    /// </summary>
    public bool IsPrimaryKey => _isPrimaryKey;
    /// <summary>
    /// Is this attribute's value nullable
    /// </summary>
    public bool IsNullable => _isNullable;
    /// <summary>
    /// Ordinal number position of the attribute
    /// </summary>
    public int Ordinal => _ordinal;
    /// <summary>
    /// Parent tableau
    /// </summary>
    public Tableau Tableau => _tableau;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Attribute name</param>
    /// <param name="dataType">Attribute data type</param>
    /// <param name="isPrimaryKey">Is this attribute part a primary key or part of a composite key</param>
    /// <param name="isNullable">Is this attribute's value nullable</param>
    /// <param name="ordinal">Ordinal number position of the attribute</param>
    /// <param name="tableau">Parent tableau</param>
    internal Attribute(string name!!, DataTypes dataType, bool isPrimaryKey, bool isNullable, int ordinal, Tableau tableau!!)
    {
        _name = name;
        _dataType = dataType;
        _isPrimaryKey = isPrimaryKey;
        _isNullable = isNullable;
        _ordinal = ordinal;
        _tableau = tableau;
    }

    public override bool Equals(object? obj)
    {
        return obj is Attribute attribute &&
               _name == attribute._name &&
               _dataType == attribute._dataType &&
               _isPrimaryKey == attribute._isPrimaryKey &&
               _isNullable == attribute._isNullable &&
               _ordinal == attribute._ordinal &&
               _tableau.Id == attribute._tableau.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_name, _dataType, _isPrimaryKey, _isNullable, _ordinal, _tableau);
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
        => $"({Name} {DataType} {Ordinal} {IsNullable} {IsPrimaryKey})";
}

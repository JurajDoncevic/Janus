using Janus.Commons.SchemaModels.Exceptions;

namespace Janus.Commons.SchemaModels.Building;

/// <summary>
/// Builder for an attribute
/// </summary>
public class AttributeBuilder : IAttributeBuilder
{
    private string _attributeName;
    private DataTypes _attributeDataType;
    private int _attributeOrdinal = -1;
    private bool _isAttributeIdentity = false;
    private bool _isAttributeNullable = true;
    private Tableau _parentTableau;
    private string _attributeDescription;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="attributeName">Attribute</param>
    /// <param name="parentTableau">Tableau inside which the builder works</param>
    internal AttributeBuilder(string attributeName, Tableau parentTableau)
    {
        if (string.IsNullOrEmpty(attributeName))
        {
            throw new ArgumentException($"'{nameof(attributeName)}' cannot be null or empty.", nameof(attributeName));
        }

        _attributeName = attributeName;
        _attributeDescription = String.Empty;
        _parentTableau = parentTableau ?? throw new ArgumentNullException(nameof(parentTableau));
    }

    /// <summary>
    /// Sets the attribute's name
    /// </summary>
    /// <param name="name">Attribute name</param>
    /// <returns></returns>
    public AttributeBuilder WithName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        }

        _attributeName = name;
        return this;
    }

    /// <summary>
    /// Sets the attribute's data type
    /// </summary>
    /// <param name="dataType">Data type</param>
    /// <returns></returns>
    public AttributeBuilder WithDataType(DataTypes dataType)
    {
        _attributeDataType = dataType;
        return this;
    }

    /// <summary>
    /// Sets the attribute's ordinal number
    /// </summary>
    /// <param name="ordinal">Ordinal number</param>
    /// <returns></returns>
    /// <exception cref="AttributeOrdinalOutOfRange"></exception>
    public AttributeBuilder WithOrdinal(int ordinal)
    {
        if (ordinal < 0)
            throw new AttributeOrdinalOutOfRange(_attributeName);

        _attributeOrdinal = ordinal;
        return this;
    }

    /// <summary>
    /// Sets whether the attribute is or part of an identity
    /// </summary>
    /// <param name="isIdentity"></param>
    /// <returns></returns>
    public AttributeBuilder WithIsIdentity(bool isIdentity)
    {
        _isAttributeIdentity = isIdentity;
        return this;
    }

    /// <summary>
    /// Sets whether the attribute value is nullable
    /// </summary>
    /// <param name="isNullable"></param>
    /// <returns></returns>
    public AttributeBuilder WithIsNullable(bool isNullable)
    {
        _isAttributeNullable = isNullable;
        return this;
    }

    /// <summary>
    /// Sets the attribute's description
    /// </summary>
    /// <param name="isNullable"></param>
    /// <returns></returns>
    public AttributeBuilder WithDescription(string attributeDescription)
    {
        _attributeDescription = attributeDescription ?? string.Empty;
        return this;
    }

    /// <summary>
    /// Builds the attribute
    /// </summary>
    /// <returns></returns>
    public Attribute Build()
    {
        return new Attribute(_attributeName, _attributeDataType, _isAttributeIdentity, _isAttributeNullable, _attributeOrdinal, _parentTableau, _attributeDescription);
    }
}


public interface IAttributeBuilder
{
    /// <summary>
    /// Sets the attribute's data type
    /// </summary>
    /// <param name="dataType">Data type</param>
    /// <returns></returns>
    AttributeBuilder WithDataType(DataTypes dataType);
    /// <summary>
    /// Sets the attribute's description
    /// </summary>
    /// <param name="isNullable"></param>
    /// <returns></returns>
    AttributeBuilder WithDescription(string attributeDescription);
    /// <summary>
    /// Sets whether the attribute value is nullable
    /// </summary>
    /// <param name="isNullable"></param>
    /// <returns></returns>
    AttributeBuilder WithIsNullable(bool isNullable);
    /// <summary>
    /// Sets whether the attribute is or part of an identity
    /// </summary>
    /// <param name="isIdentity"></param>
    /// <returns></returns>
    AttributeBuilder WithIsIdentity(bool isIdentity);
    /// <summary>
    /// Sets the attribute's name
    /// </summary>
    /// <param name="name">Attribute name</param>
    /// <returns></returns>
    AttributeBuilder WithName(string name);
    /// <summary>
    /// Sets the attribute's ordinal number
    /// </summary>
    /// <param name="ordinal">Ordinal number</param>
    /// <returns></returns>
    /// <exception cref="AttributeOrdinalOutOfRange"></exception>
    AttributeBuilder WithOrdinal(int ordinal);
    Attribute Build();
}

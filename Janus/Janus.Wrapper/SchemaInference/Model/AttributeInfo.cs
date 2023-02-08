using Janus.Commons.SchemaModels;

namespace Janus.Wrapper.SchemaInference.Model;
public class AttributeInfo
{
    private readonly string _name;
    private readonly DataTypes _dataType;
    private readonly bool _isPrimaryKey;
    private readonly bool _isNullable;
    private readonly int _ordinal;

    public AttributeInfo(string name, DataTypes dataType, bool isPrimaryKey, bool isNullable, int ordinal)
    {
        _name = name;
        _dataType = dataType;
        _isPrimaryKey = isPrimaryKey;
        _isNullable = isNullable;
        _ordinal = ordinal;
    }

    public string Name => _name;

    public DataTypes DataType => _dataType;

    public bool IsPrimaryKey => _isPrimaryKey;

    public bool IsNullable => _isNullable;

    public int Ordinal => _ordinal;
}

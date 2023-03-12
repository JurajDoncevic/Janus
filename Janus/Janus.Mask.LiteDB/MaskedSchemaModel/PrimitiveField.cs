namespace Janus.Mask.LiteDB.MaskedSchemaModel;
public class PrimitiveField : Field
{
    internal PrimitiveField(string name, FieldTypes type, bool isIdentity) : base(name, type, isIdentity)
    {
    }
}

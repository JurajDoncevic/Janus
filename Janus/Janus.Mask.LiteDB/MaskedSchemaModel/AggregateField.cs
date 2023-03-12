namespace Janus.Mask.LiteDB.MaskedSchemaModel;
public class AggregateField : Field
{
    private readonly FieldTypes _elementsType;

    public AggregateField(string name, bool isIdentity) : base(name, FieldTypes.ARRAY, isIdentity)
    {
    }

    public FieldTypes ElementsType => _elementsType;
}

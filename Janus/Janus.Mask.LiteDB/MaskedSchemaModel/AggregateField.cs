namespace Janus.Mask.LiteDB.MaskedSchemaModel;
public class AggregateField : Field
{
    private readonly FieldTypes _elementsType;

    public AggregateField(string name) : base(name, FieldTypes.ARRAY)
    {
    }

    public FieldTypes ElementsType => _elementsType;
}

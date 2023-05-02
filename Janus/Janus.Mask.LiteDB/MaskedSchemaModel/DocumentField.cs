namespace Janus.Mask.LiteDB.MaskedSchemaModel;
public sealed class DocumentField : Field
{
    private readonly Dictionary<string, Field> _fields;

    internal DocumentField(string name, IEnumerable<Field>? fields = null, bool isIdentity = false) : base(name, FieldTypes.DOCUMENT, isIdentity)
    {
        _fields = fields?.ToDictionary(f => f.Name, f => f) ?? new Dictionary<string, Field>();
    }

    public IReadOnlyList<Field> Fields => _fields.Values.ToList();

    public Field this[string fieldName] => _fields[fieldName];

    internal bool AddField(Field field)
    {
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        return _fields.TryAdd(field.Name, field);
    }

    internal bool RemoveField(string fieldName)
    {
        if(fieldName is null)
        {
            throw new ArgumentNullException(nameof(fieldName));
        }

        return _fields.Remove(fieldName);
    }
}

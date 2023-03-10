namespace Janus.Mask.LiteDB.MaskedSchemaModel;
public class Document
{
    private readonly int _index;
    private readonly Dictionary<string, Field> _fields;

    internal Document(int index, IEnumerable<Field>? fields = null)
    {
        _index = index;
        _fields = fields?.ToDictionary(f => f.Name, f => f) ?? new Dictionary<string, Field>();
    }

    public int Index => _index;

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
        if (fieldName is null)
        {
            throw new ArgumentNullException(nameof(fieldName));
        }

        return _fields.Remove(fieldName);
    }
}

using Janus.Mask.MaskedSchemaModel;

namespace Janus.Mask.LiteDB.MaskedSchemaModel;
public class Database : MaskedDataSource
{
    private readonly string _name;
    private readonly Dictionary<string, Collection> _collections;

    internal Database(string name, IEnumerable<Collection>? collections = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }

        _name = name;
        _collections = collections?.ToDictionary(c => c.Name, c => c) ?? new Dictionary<string, Collection>();
    }

    public string Name => _name;

    public IReadOnlyList<Collection> Collections => _collections.Values.ToList();

    public Collection this[string collectionName] => _collections[collectionName];

    internal bool AddCollection(Collection collection)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        return _collections.TryAdd(collection.Name, collection);
    }

    internal bool RemoveCollection(string collectionName)
    {
        return _collections.Remove(collectionName);
    }
}

namespace Janus.Mask.LiteDB.MaskedSchemaModel;
public sealed class Collection 
{
    private readonly string _name;
    private readonly List<Document> _documents;

    internal Collection(string name, IEnumerable<Document>? documents = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }

        _name = name;
        _documents = documents?.ToList() ?? new List<Document>();
    }

    public string Name => _name;

    public IReadOnlyList<Document> Documents => _documents;

    public Document this[int index] => _documents[index];

    internal bool AddDocument(Document document)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        if (_documents.Exists(d => d.Index == document.Index))
        {
            return false;
        }
        _documents.Add(document);

        return true;
    }

    internal bool RemoveDocument(Document document)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        return _documents.Remove(document);
    }

    internal bool RemoveDocumentAt(int index)
    {
        try
        {
            _documents.RemoveAt(index);
        }
        catch (Exception)
        {

            return false;
        }
        
        return true;
    }
}

using System.Runtime.InteropServices;

namespace Janus.Mask.LiteDB.MaskedSchemaModel;
public static class LiteDbSchemaModelBuilder
{
    public static DatabaseBuilder Init(string databaseName)
    {
        return new DatabaseBuilder(databaseName);
    }
}

public class DatabaseBuilder
{
    private string _databaseName;
    private Dictionary<string, Collection> _databaseCollections;

    internal DatabaseBuilder(string databaseName, IEnumerable<Collection>? databaseCollections = null)
    {
        _databaseName = databaseName;
        _databaseCollections = databaseCollections?.ToDictionary(c => c.Name, c => c) ?? new Dictionary<string, Collection>();
    }

    public DatabaseBuilder WithName(string databaseName)
    {
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentException($"'{nameof(databaseName)}' cannot be null or whitespace.", nameof(databaseName));
        }

        _databaseName = databaseName;

        return this;
    }

    public DatabaseBuilder AddCollection(string collectionName, Func<CollectionBuilder, CollectionBuilder> configuration) 
    {
        if (string.IsNullOrWhiteSpace(collectionName))
        {
            throw new ArgumentException($"'{nameof(collectionName)}' cannot be null or whitespace.", nameof(collectionName));
        }

        var builder = new CollectionBuilder(collectionName);

        var collection = configuration(builder).Build();

        _databaseCollections.Add(collection.Name, collection);

        return this;

    }

    public Database Build()
    {
        return new Database(_databaseName, _databaseCollections.Values);
    }
}

public class CollectionBuilder
{
    private string _collectionName;
    private List<Document> _collectionDocuments;

    internal CollectionBuilder(string collectionName, IEnumerable<Document>? documents = null)
    {
        _collectionName = collectionName;
        _collectionDocuments = documents?.ToList() ?? new List<Document>();
    }

    public CollectionBuilder WithName(string name) 
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }

        _collectionName = name;

        return this;
    }

    public CollectionBuilder AddDocument(Func<DocumentBuilder, DocumentBuilder> configuration)
    {
        var builder = new DocumentBuilder(_collectionDocuments.Count + 1);

        var document = configuration(builder).Build();

        _collectionDocuments.Add(document);

        return this;
    }

    internal Collection Build()
    {
        return new Collection(_collectionName, _collectionDocuments);
    }
}

public class DocumentBuilder
{
    private int _documentIndex;
    private Dictionary<string, Field> _documentFields;

    internal DocumentBuilder(int index, IEnumerable<Field>? fields = null)
    {
        _documentIndex = index;
        _documentFields = fields?.ToDictionary(f => f.Name, f => f) ?? new Dictionary<string, Field>();
    }

    public DocumentBuilder WithIndex(int index)
    {
        _documentIndex = index;

        return this;
    }

    public DocumentBuilder WithPrimitiveField(string fieldName, Func<PrimitiveFieldBuilder, PrimitiveFieldBuilder> configuration)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or whitespace.", nameof(fieldName));
        }

        var builder = new PrimitiveFieldBuilder(fieldName, FieldTypes.INT32);

        var primitiveField = configuration(builder).Build();

        _documentFields.Add(primitiveField.Name, primitiveField);

        return this;
    }

    internal Document Build()
    {
        return new Document(_documentIndex, _documentFields.Values);
    }
}


public class PrimitiveFieldBuilder
{
    private FieldTypes _fieldType;
    private string _fieldName;
    private bool _isIdentity;
    private DocumentField? _documentField;

    internal PrimitiveFieldBuilder(string fieldName, FieldTypes fieldType, bool? isIdentity = null)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or whitespace.", nameof(fieldName));
        }

        _fieldType = fieldType;
        _fieldName = fieldName;
        _isIdentity = isIdentity ?? false;
    }

    public PrimitiveFieldBuilder WithName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }

        _fieldName = name;
        return this;
    }

    public PrimitiveFieldBuilder AsIdentity()
    {
        _isIdentity = true;
        return this;
    }

    public PrimitiveFieldBuilder AsInt32()
    {
        _fieldType = FieldTypes.INT32;

        return this;
    }

    public PrimitiveFieldBuilder AsInt64()
    {
        _fieldType = FieldTypes.INT64;

        return this;
    }

    public PrimitiveFieldBuilder AsString()
    {
        _fieldType = FieldTypes.STRING;

        return this;
    }

    public PrimitiveFieldBuilder AsDouble()
    {
        _fieldType = FieldTypes.DOUBLE;

        return this;
    }

    internal PrimitiveField Build()
    {
        return new PrimitiveField(_fieldName, _fieldType);
    }
}

public class DocumentFieldBuilder
{

}
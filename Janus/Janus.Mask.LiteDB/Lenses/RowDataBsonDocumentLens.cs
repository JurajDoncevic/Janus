using Janus.Base;
using Janus.Commons.DataModels;
using Janus.Lenses;
using LiteDB;

namespace Janus.Mask.LiteDB.Lenses;
public sealed class RowDataBsonDocumentLens : AsymmetricLens<RowData, BsonDocument>
{
    private readonly string _columnNamePrefix;

    internal RowDataBsonDocumentLens(string? columnNamePrefix = null)
    {
        _columnNamePrefix = columnNamePrefix ?? string.Empty;
    }

    public override Func<BsonDocument, RowData?, RowData> Put =>
        (view, originalSource) =>
        {
            return RowData.FromDictionary(view.RawValue.ToDictionary(
                            kv => $"{_columnNamePrefix}{kv.Key}",
                            kv => GetSystemValue(kv.Value)
                        ));
        };


    private object? GetSystemValue(BsonValue bsonValue)
    => bsonValue switch
    {
        { Type: BsonType.Int32 } => bsonValue.AsInt32,
        { Type: BsonType.Int64 } => bsonValue.AsInt64,
        { Type: BsonType.Decimal } => (double)bsonValue.AsDecimal,
        { Type: BsonType.Double } => bsonValue.AsDouble,
        { Type: BsonType.Boolean } => bsonValue.AsDateTime,
        { Type: BsonType.Binary } => bsonValue.AsBinary,
        { Type: BsonType.DateTime } => bsonValue.AsDateTime,
        _ => throw new NotSupportedException()
    };


    public override Func<RowData, BsonDocument> Get =>
        (source) =>
        {
            var bsonDocument = new BsonDocument();
            foreach (var (fieldName, value) in source.ColumnValues.Map(t => (t.Key.Split('.').Last(), t.Value)))
            {
                bsonDocument.Add(fieldName, new BsonValue(value));       
            }

            return bsonDocument;
        };
}


public static class RowDataBsonDocumentLenses
{
    public static RowDataBsonDocumentLens Construct(string? columnNamePrefix = null)
        => new RowDataBsonDocumentLens(columnNamePrefix);
}
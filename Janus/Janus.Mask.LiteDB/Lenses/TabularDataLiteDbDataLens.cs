using FunctionalExtensions.Base;
using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using Janus.Lenses;
using Janus.Mask.LiteDB.MaskedDataModel;
using LiteDB;

namespace Janus.Mask.LiteDB.Lenses;
public sealed class TabularDataLiteDbDataLens : Lens<TabularData, LiteDbData>
{
    private readonly string _columnNamePrefix;

    private readonly RowDataBsonDocumentLens _rowDataLens;
    internal TabularDataLiteDbDataLens(string? columnNamePrefix = null)
    {
        _columnNamePrefix = columnNamePrefix ?? string.Empty;
        _rowDataLens = new RowDataBsonDocumentLens(columnNamePrefix);
    }

    public override Func<LiteDbData, TabularData?, TabularData> Put =>
        (view, originalSource) =>
        {
            var columnDataTypes =
                originalSource?.ColumnDataTypes.ToDictionary(_ => $"{_columnNamePrefix}{_.Key}", _ => _.Value) ??
                view.Data.FirstOrDefault()?.RawValue.ToDictionary(rv => $"{_columnNamePrefix}{rv.Key}", rv => MapToDataType(rv.Value.Type)) ??
                new Dictionary<string, DataTypes>();

            var builder = TabularDataBuilder.InitTabularData(columnDataTypes);

            foreach (var bsonDocument in view.Data)
            {
                builder = 
                    builder.AddRow(rowConf => rowConf.WithRowData(
                        new Dictionary<string, object?>(_rowDataLens.Put(bsonDocument, null).ColumnValues)
                        )
                    );
            }

            return builder.Build();
        };

    private DataTypes MapToDataType(BsonType bsonType)
        => bsonType switch
        {
            BsonType.Int32 => DataTypes.INT,
            BsonType.Int64 => DataTypes.LONGINT,
            BsonType.Decimal => DataTypes.DECIMAL,
            BsonType.Double => DataTypes.DECIMAL,
            BsonType.Boolean => DataTypes.BOOLEAN,
            BsonType.String => DataTypes.STRING,
            BsonType.DateTime => DataTypes.DATETIME,
            BsonType.Binary => DataTypes.BINARY,
            _ => throw new NotSupportedException()
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
        

    public override Func<TabularData, LiteDbData> Get =>
        (source) =>
        {
            var bsonDocuments = source.RowData.Map(rd => _rowDataLens.Get(rd));
            var view = new LiteDbData(bsonDocuments);

            return view;
        };
}

public static class TabularDataLiteDbDataLenses
{
    public static TabularDataLiteDbDataLens Construct(string? columnNamePrefix = null)
        => new TabularDataLiteDbDataLens(columnNamePrefix);
}
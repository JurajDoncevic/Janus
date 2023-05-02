using Janus.Base;
using Janus.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Mask.LiteDB.MaskedDataModel;
using Janus.Wrapper.Translation;
using LiteDB;

namespace Janus.Mask.LiteDB.Translation;
public class LiteDbDataTranslator : IMaskDataTranslator<LiteDbData, BsonDocument>
{
    public Result<TabularData> Translate(LiteDbData source)
    {
        return Results.OnException<TabularData>(new NotImplementedException());
    }

    public Result<LiteDbData> Translate(TabularData destination)
        => Results.AsResult(() =>
        {
            var bsonDocs = destination.RowData.Map(rd => BsonMapper.Global.ToDocument(rd.ColumnValues));

            return Results.OnSuccess(new LiteDbData(bsonDocs));
        });
}

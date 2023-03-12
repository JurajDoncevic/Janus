using Janus.Mask.MaskedDataModel;
using LiteDB;

namespace Janus.Mask.LiteDB.MaskedDataModel;
public class LiteDbData : MaskedData<BsonDocument>
{
    public override IReadOnlyList<BsonDocument> Data => _items;

    public override long ItemCount => _items.LongCount();

    private readonly List<BsonDocument> _items;

    public LiteDbData(IEnumerable<BsonDocument> items)
    {
        _items = items?.ToList() ?? new List<BsonDocument>();
    }
}

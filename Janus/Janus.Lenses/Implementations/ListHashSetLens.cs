using Janus.Base;

namespace Janus.Lenses.Implementations;
public class ListHashSetLens<LItem, RItem> : SymmetricLens<List<LItem>, HashSet<RItem>>
{
    private readonly SymmetricLens<LItem, RItem> _itemLens;
    protected override Result<List<LItem>> _CreateLeft(Option<HashSet<RItem>> right)
        => Results.AsResult(() =>
            right.Match(
                hashSet => hashSet.Fold(
                    new List<Result<LItem>>(),
                    (rItem, list) => list.Pass(lst => lst.Add(_itemLens.PutLeft(rItem, Option<LItem>.None)))
                    ),
                () => new List<Result<LItem>>()
                )
            .AutoFold()
            .Map(Enumerable.ToList)
        );

    protected override Result<HashSet<RItem>> _CreateRight(Option<List<LItem>> left)
        => Results.AsResult(() => 
            left.Match(
                list => list.Fold(
                    new HashSet<Result<RItem>>()
                    )
                )
        );

    protected override Result<List<LItem>> _PutLeft(HashSet<RItem> right, Option<List<LItem>> left)
    {
        throw new NotImplementedException();
    }

    protected override Result<HashSet<RItem>> _PutRight(List<LItem> left, Option<HashSet<RItem>> right)
    {
        throw new NotImplementedException();
    }
}

using Janus.Base;
using System.Net.Http.Headers;

namespace Janus.Lenses.Implementations;
public class ListHashSetLens<TLItem, TRItem> : SymmetricLens<List<TLItem>, HashSet<TRItem>>
{
    private readonly SymmetricLens<TLItem, TRItem> _itemLens;

    internal ListHashSetLens(SymmetricLens<TLItem, TRItem> itemLens)
    {
        _itemLens = itemLens;
    }

    protected override Result<List<TLItem>> _CreateLeft(Option<HashSet<TRItem>> right)
        => Results.AsResult(() =>
            right.Match(
                hashSet => hashSet.Fold(
                    new List<Result<TLItem>>(),
                    (rItem, list) => list.Pass(l => l.Add(_itemLens.PutLeft(rItem, Option<TLItem>.None)))
                    ),
                () => new List<Result<TLItem>>()
                )
            .AutoFold()
            .Map(Enumerable.ToList)
        );

    protected override Result<HashSet<TRItem>> _CreateRight(Option<List<TLItem>> left)
        => Results.AsResult(() =>
            left.Match(
                list => list.Fold(
                    new HashSet<Result<TRItem>>(),
                    (lItem, set) => set.Append(_itemLens.PutRight(lItem, Option<TRItem>.None)).ToHashSet()
                    ),
                () => new HashSet<Result<TRItem>>()
                )
            .AutoFold()
            .Map(Enumerable.ToHashSet)
        );

    protected override Result<List<TLItem>> _PutLeft(HashSet<TRItem> right, Option<List<TLItem>> left)
        => Results.AsResult(() =>
        {
            var trivialView = CreateRight(left).Match(r => r, _ => new HashSet<TRItem>());
            var positionMap = left.Value.Map(li =>
                                            right
                                            .Mapi((ridx, ri) => (ridx, ri))
                                            .FirstOrDefault(rt => LensedEquals(rt.ri, li)))
                                        .Mapi((lidx, t) => (lidx, t.ridx))
                                        .ToDictionary(_ => (int)_.lidx, _ => (int)_.ridx);
            var revMapping = (int setIdx) => positionMap.FirstOrDefault(kv => kv.Value == setIdx).Key;

            var items = right.Mapi((ridx, ritem) => _itemLens.PutLeft(ritem, left.Map(ls => ls[revMapping((int)ridx)])));

            var list = 
                Enumerable.Range(0, positionMap.Count)
                .Map(idx => items.ElementAt(positionMap[idx]))
                .ToList();

            return list
                .AutoFold()
                .Map(Enumerable.ToList);
        });


    protected override Result<HashSet<TRItem>> _PutRight(List<TLItem> left, Option<HashSet<TRItem>> right)
    {
        return left.Map(li => _itemLens.PutRight(li, right.Map(rs => rs.FirstOrDefault(r => LensedEquals(r, li)))))
                   .AutoFold()
                   .Map(Enumerable.ToHashSet);
    }

    private Result LensedEquals(TLItem lItem, TRItem rItem)
        => _itemLens.PutRight(lItem, Option<TRItem>.Some(rItem)).Bind(li => li.Equals(rItem));

    private Result LensedEquals(TRItem rItem, TLItem lItem)
        => _itemLens.PutLeft(rItem, Option<TLItem>.Some(lItem)).Bind(ri => ri.Equals(lItem));
}

public static class ListHashSetLenses
{
    public static ListHashSetLens<TLItem, TRItem> Construct<TLItem, TRItem>(SymmetricLens<TLItem, TRItem> itemLens)
        => new ListHashSetLens<TLItem, TRItem>(itemLens);
}
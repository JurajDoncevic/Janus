using Janus.Lenses.Implementations;

namespace Janus.Lenses.Tests.Implementations;
public sealed class ListHashSetLensTests : SymmetricLensTestingFramework<List<int>, HashSet<string>>
{
    protected override List<int> _x => new List<int> { 1, 1, 2, 3, 4, 5, 5, 6, 7, 7 };

    protected override HashSet<string> _y => new HashSet<string> { "1", "2", "3", "4", "5", "6", "7" };

    protected override SymmetricLens<List<int>, HashSet<string>> _lens 
        => ListHashSetLenses.Construct(IntStringLenses.Construct());
}

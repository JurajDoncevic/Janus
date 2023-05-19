using Janus.Lenses.Implementations;

namespace Janus.Lenses.Tests.Implementations;
public sealed class IntStringLensTests : SymmetricLensTestingFramework<int, string>
{
    protected override int _x => -42;

    protected override string _y => "-42";

    protected override SymmetricLens<int, string> _lens => IntStringLenses.Construct();
}

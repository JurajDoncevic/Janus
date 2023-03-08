using Janus.Lenses.Implementations;

namespace Janus.Lenses.Tests.Implementations;
public class IntStringLensTests : BaseLensTesting
{
    public override void GetPutTest()
    {
        int source = -42;

        var lens = IntStringLenses.Construct();

        var outputSource = lens.Put(lens.Get(source), source);

        Assert.Equal(source, outputSource);
    }

    public override void PutGetTest()
    {
        int source = -42;
        string view = "-42";

        var lens = IntStringLenses.Construct();

        var outputView = lens.Get(lens.Put(view, source));

        Assert.Equal(view, outputView);
    }

    public override void PutPutTest()
    {
        int source = -42;
        string view = "-42";
        string updatedView = "92";

        var lens = IntStringLenses.Construct();

        var left = lens.Put(updatedView, lens.Put(view, source));
        var right = lens.Put(updatedView, source);

        Assert.Equal(left, right);
    }
}

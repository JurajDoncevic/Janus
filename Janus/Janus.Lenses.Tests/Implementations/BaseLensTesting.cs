using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;

namespace Janus.Lenses.Tests.Implementations;

public abstract class BaseLensTesting
{
    /// <summary>
    /// Should confirm that get(put v s) = v
    /// </summary>
    [Fact(DisplayName = $"PUTGET rule test")]
    public abstract void PutGetTest();

    /// <summary>
    /// Should confirm that put(get s) s = s
    /// </summary>
    [Fact(DisplayName = $"GETPUT rule test")]
    public abstract void GetPutTest();

    /// <summary>
    /// Should confirm that put v' (put v s) = put v' s
    /// </summary>
    [Fact(DisplayName = $"PUTPUT rule test")]
    public abstract void PutPutTest();
}
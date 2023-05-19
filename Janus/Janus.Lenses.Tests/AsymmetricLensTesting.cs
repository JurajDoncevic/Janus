using Janus.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using Janus.Lenses.Implementations;

namespace Janus.Lenses.Tests;

public abstract class AsymmetricLensTesting
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
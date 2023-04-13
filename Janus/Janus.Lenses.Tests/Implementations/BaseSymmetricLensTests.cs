using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Lenses.Tests.Implementations;
public abstract class BaseSymmetricLensTests
{
    /// <summary>
    /// Should confirm that putL x (createR x) = x
    /// </summary>
    [Fact(DisplayName = $"CreatePutRL rule test")]
    public abstract void CreatePutRLTest();

    /// <summary>
    /// Should confirm that putR y (createL y) = y
    /// </summary>
    [Fact(DisplayName = $"CreatePutLR rule test")]
    public abstract void CreatePutLRTest();

    /// <summary>
    /// Should confirm that putL x (putR y x) = x 
    /// </summary>
    [Fact(DisplayName = $"PutRL rule test")]
    public abstract void PutRLTest();

    /// <summary>
    /// Should confirm that putR y (putL x y) = y
    /// </summary>
    [Fact(DisplayName = $"PutLR rule test")]
    public abstract void PutLRTest();
}

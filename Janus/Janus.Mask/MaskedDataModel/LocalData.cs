namespace Janus.Mask.MaskedDataModel;

/// <summary>
/// Describes a mask's local data model
/// </summary>
/// <typeparam name="TItem">Data item type</typeparam>
public abstract class MaskedData<TItem>
{
    /// <summary>
    /// List of data items
    /// </summary>
    public abstract IReadOnlyList<TItem> Data { get; }

    /// <summary>
    /// Amount of data items
    /// </summary>
    public abstract long ItemCount { get; }
}


namespace Janus.Commons.SchemaModels.Exceptions;

/// <summary>
/// Exception that is thrown when an attribute with the same name already exists in a tableau
/// </summary>
public class UpdateSetsOverlapException : Exception
{
    internal UpdateSetsOverlapException(HashSet<UpdateSet> updateSets, string tableauName)
        : base($"Update sets {updateSets} have an overlap in {tableauName}.")
    {

    }
}

using Janus.Commons.SchemaModels;

namespace Janus.Commons.CommandModels.Exceptions;
[Serializable]
internal class NoUpdateSetFoundForExpressionAttributesException : Exception
{
    public NoUpdateSetFoundForExpressionAttributesException(HashSet<AttributeId> attributeIds, Tableau referencedTableau)
        : base($"No valid update set found on tableau {referencedTableau.Id} for referenced attributes ({string.Join(", ", attributeIds)})")
    { }    
    public NoUpdateSetFoundForExpressionAttributesException(HashSet<string> attributeNames, Tableau referencedTableau)
        : base($"No valid update set found on tableau {referencedTableau.Id} for referenced attributes ({string.Join(", ", attributeNames)})")
    { }
}
using Janus.Commons.SchemaModels;

namespace Janus.Commons.CommandModels.Exceptions;

public class AttributeNotInTargetTableauException : Exception
{
    public AttributeNotInTargetTableauException(string invalidAttributeId, TableauId tableauId)
        : base($"Invalid attribute reference {invalidAttributeId} for tableau {tableauId}")
    {
    }
}
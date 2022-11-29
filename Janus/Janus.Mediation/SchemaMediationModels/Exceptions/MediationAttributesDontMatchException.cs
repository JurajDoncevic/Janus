using Janus.Commons.SchemaModels;

namespace Janus.Mediation.SchemaMediationModels.Exceptions;

public class MediationAttributesDontMatchException : Exception
{
    public MediationAttributesDontMatchException(IEnumerable<AttributeId> sourceQueryAttrs, IEnumerable<string> declaredAttrs) 
        : base($"Declared attributes in mediation ({string.Join(", ", declaredAttrs)}) don't match projection clause of source query ({string.Join(", ", sourceQueryAttrs)})")
    {
    }
}
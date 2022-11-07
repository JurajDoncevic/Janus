namespace Janus.Mediation.MediationModels.Exceptions;

public class MediationAttributesDontMatchException : Exception
{
    public MediationAttributesDontMatchException(IEnumerable<string> sourceQueryAttrs, IEnumerable<string> declaredAttrs) 
        : base($"Declared attributes in mediation ({string.Join(", ", declaredAttrs)}) don't match projection clause of source query ({string.Join(", ", sourceQueryAttrs)})")
    {
    }
}
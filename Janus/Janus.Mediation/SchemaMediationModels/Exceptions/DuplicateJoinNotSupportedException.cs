using Janus.Mediation.SchemaMediationModels.MediationQueryModels;

namespace Janus.Mediation.SchemaMediationModels.Exceptions;

public class DuplicateJoinNotSupportedException : Exception
{
    public DuplicateJoinNotSupportedException(Join join)
        : base($"Duplicate join not supported. Duplicate detected on join of tableaus {join.ForeignKeyTableauId} and {join.PrimaryKeyTableauId} with {join.ForeignKeyAttributeId}-{join.PrimaryKeyAttributeId}.")
    {
    }
}

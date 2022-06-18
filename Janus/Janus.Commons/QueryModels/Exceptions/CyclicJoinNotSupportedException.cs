namespace Janus.Commons.QueryModels.Exceptions;

public class CyclicJoinNotSupportedException : Exception
{
    public CyclicJoinNotSupportedException(Join join)
        : base($"Cyclic joins are not supported. Join cycle detected adding join between tableaus {join.ForeignKeyTableauId} and {join.PrimaryKeyTableauId} with: {join.ForeignKeyAttributeId}-{join.PrimaryKeyAttributeId}")
    {
    }
}

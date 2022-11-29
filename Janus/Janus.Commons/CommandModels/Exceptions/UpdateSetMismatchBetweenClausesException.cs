using Janus.Commons.SchemaModels;

namespace Janus.Commons.CommandModels.Exceptions;
internal class UpdateSetMismatchBetweenClausesException : Exception
{
    public UpdateSetMismatchBetweenClausesException(UpdateSet? updateSet1, UpdateSet? updateSet2, TableauId tableauId)
        : base($"Clauses in a command referenced different update sets on tableau {tableauId}: {updateSet1?.ToString() ?? "()"}, {updateSet2?.ToString() ?? "()"}")
    {
    }
}

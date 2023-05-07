using Janus.Communication.Nodes.Implementations;
using Janus.Logging;
using Janus.Mask.Sqlite.MaskedSchemaModel;
using Janus.Mask.Sqlite.Translation;

namespace Janus.Mask.Sqlite;
public sealed class SqliteMaskSchemaManager : MaskSchemaManager<Database>
{
    public SqliteMaskSchemaManager(MaskCommunicationNode communicationNode, SqliteSchemaTranslator schemaTranslator, ILogger? logger = null) : base(communicationNode, schemaTranslator, logger)
    {
    }
}

using Janus.Communication.Nodes.Implementations;
using Janus.Logging;
using Janus.Mask.LiteDB.MaskedSchemaModel;
using Janus.Mask.LiteDB.Translation;

namespace Janus.Mask.LiteDB;

public class LiteDbMaskSchemaManager : MaskSchemaManager<Database>
{
    public LiteDbMaskSchemaManager(MaskCommunicationNode communicationNode, LiteDbSchemaTranslator schemaTranslator, ILogger? logger = null) : base(communicationNode, schemaTranslator, logger)
    {
    }
}
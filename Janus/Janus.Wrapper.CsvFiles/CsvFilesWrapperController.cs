using Janus.Communication.Nodes.Implementations;
using Janus.Utils.Logging;
using Janus.Wrapper.Core;
using Janus.Wrapper.CsvFiles.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.CsvFiles;
public class CsvFilesWrapperController : WrapperController<Query, Selection, Joining, Projection>
{
    public CsvFilesWrapperController(WrapperCommunicationNode communicationNode, WrapperCommandManager commandManager, WrapperQueryManager<Query, Selection, Joining, Projection> queryManager, WrapperSchemaManager schemaManager, ILogger? logger = null) : base(communicationNode, commandManager, queryManager, schemaManager, logger)
    {
    }
}

using Janus.Commons.Messages;
using Janus.Serialization.Bson.SchemaModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Serialization.Bson.Messages.DTOs;
internal class SchemaResMessageDto : BaseMessageDto
{
    public new string Preamble { get => Preambles.SCHEMA_RESPONSE; }
    public DataSourceDto DataSource { get; set; }
}

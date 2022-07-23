using Janus.Serialization.MongoBson.SchemaModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Serialization.MongoBson.Messages.DTOs;
internal class SchemaResMessageDto : BaseMessageDto
{
    public DataSourceDto DataSource { get; set; }
}

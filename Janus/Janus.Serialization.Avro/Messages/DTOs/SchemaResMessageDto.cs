using Janus.Serialization.Avro.SchemaModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Serialization.Avro.Messages.DTOs;
internal class SchemaResMessageDto
{
    public string Preamble { get; set; }
    public string ExchangeId { get; set; }
    public string NodeId { get; set; }
    public DataSourceDto DataSource { get; set; }
}

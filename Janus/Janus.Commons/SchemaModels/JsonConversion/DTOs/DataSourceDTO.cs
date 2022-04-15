using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SchemaModels.JsonConversion.DTOs;

internal class DataSourceDTO
{
    public string Name { get; set; }
    public List<SchemaDTO> Schemas { get; set; }
}

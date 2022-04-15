using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SchemaModels.JsonConversion.DTOs;

internal class TableauDTO
{
    public string Name { get; set; }
    public List<AttributeDTO> Attributes { get; set; }
}

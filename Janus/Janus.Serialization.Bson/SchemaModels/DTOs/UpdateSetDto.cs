using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Serialization.Bson.SchemaModels.DTOs;
/// <summary>
/// DTO representation of an UpdateSet
/// </summary>
internal class UpdateSetDto
{
    public HashSet<string> AttributeIds { get; set; } = new HashSet<string>();
}

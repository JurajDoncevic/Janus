using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels.JsonConversion.DTOs
{
    public class ProjectionDTO
    {
        public HashSet<string> AttributeIds { get; set; } = new();
    }
}

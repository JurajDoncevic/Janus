using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.SelectionExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels.JsonConversion.DTOs
{
    public class SelectionDTO
    {
        public SelectionExpression Expression { get; set; } = TRUE();
    }
}

using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.SelectionExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels.JsonConversion.DTOs
{
    public class CommandSelectionDto
    {
        public SelectionExpression SelectionExpression { get; set; } = TRUE();
    }
}

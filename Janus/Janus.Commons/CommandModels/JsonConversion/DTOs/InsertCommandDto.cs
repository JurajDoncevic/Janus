using Janus.Commons.DataModels.JsonConversion.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels.JsonConversion.DTOs
{
    public class InsertCommandDto
    {
        public string OnTableauId { get; set; } = string.Empty;
        public TabularDataDto Instantiation { get; set; } = new();
    }
}

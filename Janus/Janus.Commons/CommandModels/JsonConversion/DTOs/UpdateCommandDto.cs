using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels.JsonConversion.DTOs
{
    public class UpdateCommandDto
    {
        public string OnTableauId { get; set; } = String.Empty;
        public Dictionary<string, (object?, Type)> Mutation { get; set; } = new ();

        public CommandSelectionDto Selection { get; set; } = new CommandSelectionDto();
    }
}

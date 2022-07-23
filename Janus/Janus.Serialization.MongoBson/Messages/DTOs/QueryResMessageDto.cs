using Janus.Serialization.MongoBson.DataModels.DTOs;
using Janus.Serialization.MongoBson.QueryModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Serialization.MongoBson.Messages.DTOs;
internal class QueryResMessageDto : BaseMessageDto
{
    public TabularDataDto? TabularData { get; set; }
    public string ErrorMessage { get; set; }
    public int BlockNumber { get; set; }
    public int TotalBlocks { get; set; }
}

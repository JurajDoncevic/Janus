using Janus.Serialization.MongoBson.QueryModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Serialization.MongoBson.Messages.DTOs;
internal class QueryReqMessageDto : BaseMessageDto
{
    public QueryDto Query { get; set; }
}

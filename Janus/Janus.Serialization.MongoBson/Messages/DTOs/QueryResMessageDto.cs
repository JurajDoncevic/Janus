using Janus.Serialization.MongoBson.DataModels.DTOs;

namespace Janus.Serialization.MongoBson.Messages.DTOs;
internal class QueryResMessageDto : BaseMessageDto
{
    public TabularDataDto? TabularData { get; set; }
    public string ErrorMessage { get; set; }
    public int BlockNumber { get; set; }
    public int TotalBlocks { get; set; }
}

using ProtoBuf;

namespace Janus.Serialization.Protobufs.QueryModels.DTOs;

[ProtoContract]
internal class QueryDto
{
    [ProtoMember(1)]
    public string OnTableauId { get; set; }

    [ProtoMember(2)]
    public List<JoinDto> Joining { get; set; }

    [ProtoMember(3)]
    public SelectionDto Selection { get; set; }

    [ProtoMember(4)]
    public ProjectionDto Projection { get; set; }
}

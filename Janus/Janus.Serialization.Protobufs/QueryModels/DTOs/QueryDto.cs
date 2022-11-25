using ProtoBuf;

namespace Janus.Serialization.Protobufs.QueryModels.DTOs;

[ProtoContract]
internal sealed class QueryDto
{
    [ProtoMember(1)]
    public string Name { get; set; } 

    [ProtoMember(2)]
    public string OnTableauId { get; set; }

    [ProtoMember(3)]
    public List<JoinDto> Joining { get; set; }

    [ProtoMember(4)]
    public SelectionDto Selection { get; set; }

    [ProtoMember(5)]
    public ProjectionDto Projection { get; set; }
}

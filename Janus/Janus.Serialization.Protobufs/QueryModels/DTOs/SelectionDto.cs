using ProtoBuf;

namespace Janus.Serialization.Protobufs.QueryModels.DTOs;

[ProtoContract]
internal sealed class SelectionDto
{
    [ProtoMember(1)]
    public string Expression { get; set; } = "TRUE";
}

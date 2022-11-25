using Janus.Serialization.Protobufs.DataModels.DTOs;
using ProtoBuf;

namespace Janus.Serialization.Protobufs.CommandModels.DTOs;

[ProtoContract]
internal sealed class InsertCommandDto
{
    [ProtoMember(1)]
    public string OnTableauId { get; set; } = string.Empty;

    [ProtoMember(2)]
    public TabularDataDto Instantiation { get; set; } = new();
}

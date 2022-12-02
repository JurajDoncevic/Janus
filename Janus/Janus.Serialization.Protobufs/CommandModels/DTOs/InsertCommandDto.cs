using Janus.Serialization.Protobufs.DataModels.DTOs;
using ProtoBuf;

namespace Janus.Serialization.Protobufs.CommandModels.DTOs;

[ProtoContract]
internal sealed class InsertCommandDto
{ 

    [ProtoMember(1)]
    public string Name { get; set; }

    [ProtoMember(2)]
    public string OnTableauId { get; set; } = string.Empty;

    [ProtoMember(3)]
    public TabularDataDto Instantiation { get; set; } = new();

    public InsertCommandDto()
    {

    }

    public InsertCommandDto(string onTableauId, TabularDataDto instantiation, string name)
    {
        Name = name;
        OnTableauId = onTableauId;
        Instantiation = instantiation;
    }
}

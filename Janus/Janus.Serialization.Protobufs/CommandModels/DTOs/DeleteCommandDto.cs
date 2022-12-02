using ProtoBuf;

namespace Janus.Serialization.Protobufs.CommandModels.DTOs;

[ProtoContract]
internal sealed class DeleteCommandDto
{
    [ProtoMember(1)]
    public string Name { get; set; }
    
    [ProtoMember(2)]
    public string OnTableauId { get; set; } = String.Empty;

    [ProtoMember(3)]
    public CommandSelectionDto? Selection { get; set; } = new CommandSelectionDto();

    public DeleteCommandDto()
    {

    }

    public DeleteCommandDto(string onTableauId, CommandSelectionDto? selection, string name)
    {
        OnTableauId = onTableauId;
        Selection = selection;
        Name = name;
    }
}

using ProtoBuf;

namespace Janus.Serialization.Protobufs.CommandModels.DTOs;

[ProtoContract]
internal class DeleteCommandDto
{
    [ProtoMember(1)]
    public string OnTableauId { get; set; } = String.Empty;

    [ProtoMember(2)]
    public CommandSelectionDto? Selection { get; set; } = new CommandSelectionDto();

    public DeleteCommandDto()
    {

    }

    public DeleteCommandDto(string onTableauId, CommandSelectionDto? selection)
    {
        OnTableauId = onTableauId;
        Selection = selection;
    }
}

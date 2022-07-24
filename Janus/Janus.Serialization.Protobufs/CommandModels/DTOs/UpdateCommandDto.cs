using Janus.Serialization.Protobufs.DataModels.DTOs;
using Janus.Serialization.Protobufs.QueryModels.DTOs;
using ProtoBuf;

namespace Janus.Serialization.Protobufs.CommandModels.DTOs;

[ProtoContract]
internal class UpdateCommandDto
{
    [ProtoMember(1)]
    public string OnTableauId { get; set; } = String.Empty;

    [ProtoMember(2)]
    public Dictionary<string, DataBytesDto> Mutation { get; set; } = new();

    [ProtoMember(3)]
    public Dictionary<string, string> MutationTypes { get; set; } = new();

    [ProtoMember(4)]
    public CommandSelectionDto? Selection { get; set; } = null;

    public UpdateCommandDto()
    {

    }

    public UpdateCommandDto(string onTableauId, Dictionary<string, DataBytesDto> mutation, Dictionary<string, string> mutationTypes, CommandSelectionDto? selection)
    {
        OnTableauId = onTableauId;
        Mutation = mutation;
        MutationTypes = mutationTypes;
        Selection = selection;
    }

}

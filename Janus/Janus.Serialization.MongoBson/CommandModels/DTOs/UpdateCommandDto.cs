namespace Janus.Serialization.MongoBson.CommandModels.DTOs;

internal class UpdateCommandDto
{
    public string OnTableauId { get; set; } = String.Empty;
    public Dictionary<string, byte[]?> Mutation { get; set; } = new();

    public Dictionary<string, string> MutationTypes { get; set; } = new();

    public CommandSelectionDto? Selection { get; set; } = null;

    public UpdateCommandDto(string onTableauId, Dictionary<string, byte[]?> mutation, Dictionary<string, string> mutationTypes, CommandSelectionDto? selection)
    {
        OnTableauId = onTableauId;
        Mutation = mutation;
        MutationTypes = mutationTypes;
        Selection = selection;
    }

}

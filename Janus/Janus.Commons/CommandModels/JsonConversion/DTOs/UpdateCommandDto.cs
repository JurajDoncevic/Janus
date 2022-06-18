namespace Janus.Commons.CommandModels.JsonConversion.DTOs;

public class UpdateCommandDto
{
    public string OnTableauId { get; set; } = String.Empty;
    public Dictionary<string, object?> Mutation { get; set; } = new();

    public Dictionary<string, string> MutationTypes { get; set; } = new();

    public CommandSelectionDto? Selection { get; set; } = null;

    public UpdateCommandDto(string onTableauId, Dictionary<string, object?> mutation, CommandSelectionDto? selection)
    {
        OnTableauId = onTableauId;
        Mutation = mutation;
        MutationTypes = mutation.ToDictionary(m => m.Key, m => m.Value?.GetType().FullName ?? typeof(object).FullName);
        Selection = selection;
    }

}

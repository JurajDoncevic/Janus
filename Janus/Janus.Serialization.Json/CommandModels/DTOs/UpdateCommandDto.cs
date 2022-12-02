namespace Janus.Serialization.Json.CommandModels.DTOs;
public class UpdateCommandDto
{
    public string Name { get; set; }
    public string OnTableauId { get; set; } = String.Empty;
    public Dictionary<string, object?> Mutation { get; set; } = new();

    public Dictionary<string, string> MutationTypes { get; set; } = new();

    public CommandSelectionDto? Selection { get; set; } = null;

    public UpdateCommandDto(string onTableauId, Dictionary<string, object?> mutation, CommandSelectionDto? selection, string name)
    {
        OnTableauId = onTableauId;
        Mutation = mutation;
        MutationTypes = mutation.ToDictionary(m => m.Key, m => m.Value?.GetType().FullName ?? typeof(object).FullName);
        Selection = selection;
        Name = name;
    }

}

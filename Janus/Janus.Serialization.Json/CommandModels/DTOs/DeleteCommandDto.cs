namespace Janus.Serialization.Json.CommandModels.DTOs;

public class DeleteCommandDto
{
    public string Name { get; set; }
    public string OnTableauId { get; set; } = String.Empty;

    public CommandSelectionDto? Selection { get; set; } = null;

    public DeleteCommandDto(string onTableauId, CommandSelectionDto? selection, string name)
    {
        OnTableauId = onTableauId;
        Selection = selection;
        Name = name;
    }
}

namespace Janus.Serialization.Avro.CommandModels.DTOs;

internal class DeleteCommandDto
{
    public string OnTableauId { get; set; } = String.Empty;

    public CommandSelectionDto? Selection { get; set; } = null;

    public DeleteCommandDto(string onTableauId, CommandSelectionDto? selection)
    {
        OnTableauId = onTableauId;
        Selection = selection;
    }
}

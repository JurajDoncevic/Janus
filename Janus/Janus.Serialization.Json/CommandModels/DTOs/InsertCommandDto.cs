using Janus.Serialization.Json.DataModels.DTOs;

namespace Janus.Serialization.Json.CommandModels.DTOs;

public class InsertCommandDto
{
    public string OnTableauId { get; set; } = string.Empty;
    public TabularDataDto Instantiation { get; set; } = new();
}

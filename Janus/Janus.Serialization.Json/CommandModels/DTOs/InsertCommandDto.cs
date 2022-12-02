using Janus.Serialization.Json.DataModels.DTOs;

namespace Janus.Serialization.Json.CommandModels.DTOs;

public class InsertCommandDto
{
    public string Name { get; set; }
    public string OnTableauId { get; set; } = string.Empty;
    public TabularDataDto Instantiation { get; set; } = new();

    public InsertCommandDto(string onTableauId, TabularDataDto instantiation, string name)
    {
        Name = name;
        OnTableauId = onTableauId;
        Instantiation = instantiation;
    }
}

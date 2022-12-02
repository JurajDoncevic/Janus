using Janus.Serialization.Bson.DataModels.DTOs;

namespace Janus.Serialization.Bson.CommandModels.DTOs;

internal sealed class InsertCommandDto
{
    public string Name { get; set; } = string.Empty;
    public string OnTableauId { get; set; } = string.Empty;
    public TabularDataDto Instantiation { get; set; } = new();

    public InsertCommandDto(string onTableauId, TabularDataDto instantiation, string name)
    {
        Name = name;
        OnTableauId = onTableauId;
        Instantiation = instantiation;
    }
}

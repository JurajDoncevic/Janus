using Janus.Serialization.Avro.DataModels.DTOs;

namespace Janus.Serialization.Avro.CommandModels.DTOs;

internal sealed class InsertCommandDto
{
    public InsertCommandDto(string onTableauId, TabularDataDto instantiation, string name)
    {
        Name = name;
        OnTableauId = onTableauId;
        Instantiation = instantiation;
    }

    public string Name { get; set; } = string.Empty;
    public string OnTableauId { get; set; } = string.Empty;
    public TabularDataDto Instantiation { get; set; } = new();
}

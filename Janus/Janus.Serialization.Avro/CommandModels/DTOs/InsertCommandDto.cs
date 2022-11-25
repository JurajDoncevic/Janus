using Janus.Serialization.Avro.DataModels.DTOs;

namespace Janus.Serialization.Avro.CommandModels.DTOs;

internal sealed class InsertCommandDto
{
    public string OnTableauId { get; set; } = string.Empty;
    public TabularDataDto Instantiation { get; set; } = new();
}

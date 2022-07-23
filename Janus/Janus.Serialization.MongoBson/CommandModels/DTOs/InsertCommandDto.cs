using Janus.Serialization.MongoBson.DataModels.DTOs;

namespace Janus.Serialization.MongoBson.CommandModels.DTOs;

internal class InsertCommandDto
{
    public string OnTableauId { get; set; } = string.Empty;
    public TabularDataDto Instantiation { get; set; } = new();
}

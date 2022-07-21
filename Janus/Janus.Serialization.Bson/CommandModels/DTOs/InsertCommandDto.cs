using Janus.Serialization.Bson.DataModels.DTOs;

namespace Janus.Serialization.Bson.CommandModels.DTOs;

public class InsertCommandDto
{
    public string OnTableauId { get; set; } = string.Empty;
    public TabularDataDto Instantiation { get; set; } = new();
}

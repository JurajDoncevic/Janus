using Janus.Commons.DataModels.JsonConversion.DTOs;

namespace Janus.Commons.CommandModels.JsonConversion.DTOs;

public class InsertCommandDto
{
    public string OnTableauId { get; set; } = string.Empty;
    public TabularDataDto Instantiation { get; set; } = new();
}

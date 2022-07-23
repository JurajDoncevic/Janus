namespace Janus.Serialization.MongoBson.QueryModels.DTOs;

internal class QueryDto
{
    public string OnTableauId { get; set; }
    public List<JoinDto> Joining { get; set; }
    public SelectionDto Selection { get; set; }
    public ProjectionDto Projection { get; set; }
}

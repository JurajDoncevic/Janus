namespace Janus.Commons.QueryModels.JsonConversion.DTOs;

public class QueryDTO
{
    public string OnTableauId { get; set; }
    public List<JoinDTO> Joining { get; set; }
    public SelectionDTO Selection { get; set; }
    public ProjectionDTO Projection { get; set; }
}

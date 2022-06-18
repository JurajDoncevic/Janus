namespace Janus.Commons.QueryModels.JsonConversion.DTOs;

public class JoinDTO
{
    public string PrimaryKeyAttributeId { get; set; } = string.Empty;
    public string PrimaryKeyTableauId { get; set; } = string.Empty;
    public string ForeignKeyAttributeId { get; set; } = string.Empty;
    public string ForeignKeyTableauId { get; set; } = string.Empty;
}

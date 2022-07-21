namespace Janus.Serialization.Json.QueryModels.DTOs;

internal class JoinDto
{
    public string PrimaryKeyAttributeId { get; set; } = string.Empty;
    public string PrimaryKeyTableauId { get; set; } = string.Empty;
    public string ForeignKeyAttributeId { get; set; } = string.Empty;
    public string ForeignKeyTableauId { get; set; } = string.Empty;
}

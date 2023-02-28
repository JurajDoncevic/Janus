namespace Janus.Mask.WebApi.InstanceManagement;
public class WebApiOptions
{
    public int ListenPort { get; init; }
    public int? ListenPortSecure { get; init; }
    public bool UseSSL { get; init; }
}

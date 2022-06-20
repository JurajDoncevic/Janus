using Janus.Mediator.Console.Options;

namespace Janus.Mediator.Console;
public class ApplicationConfigurationOptions
{
    public bool StartWithCLI { get; set; }
}

public static partial class ConfigurationOptionsExtensions
{
    public static ApplicationOptions ToApplicationOptions(this ApplicationConfigurationOptions configurationOptions)
        => new ApplicationOptions() { StartWithCLI = configurationOptions.StartWithCLI };
}
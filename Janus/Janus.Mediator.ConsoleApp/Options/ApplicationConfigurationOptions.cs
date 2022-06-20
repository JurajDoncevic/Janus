using Janus.Mediator.ConsoleApp.Options;

namespace Janus.Mediator.ConsoleApp;
public class ApplicationConfigurationOptions
{
    public bool StartWithCLI { get; set; }
}

public static partial class ConfigurationOptionsExtensions
{
    public static ApplicationOptions ToApplicationOptions(this ApplicationConfigurationOptions configurationOptions)
        => new ApplicationOptions() { StartWithCLI = configurationOptions.StartWithCLI };
}
namespace Janus.Mediator.Console;
public class ApplicationConfigurationOptions
{
    public bool StartWithCLI { get; set; }
}

public static class ApplicationCnfigurationExtensions
{
    public static ApplicationOptions ToApplicationOptions(this ApplicationConfigurationOptions configurationOptions)
        => new ApplicationOptions() { StartWithCLI = configurationOptions.StartWithCLI };
}
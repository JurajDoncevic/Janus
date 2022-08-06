using CommandLine;

namespace Janus.Wrapper.Sqlite.ConsoleApp;
internal class ApplicationOptions
{
    [Option("ui", Default = false, HelpText = "Starts mediator application with the user interface")]
    public bool StartWithUserInterface { get; init; }

    [Option("settings", Default = "settings.json", HelpText = "Points to the settings JSON file")]
    public string SettingsFilePath { get; init; } = string.Empty;
}

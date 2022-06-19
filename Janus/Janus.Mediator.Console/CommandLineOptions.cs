using CommandLine.Text;

namespace Janus.Mediator.Console;
public class CommandLineOptions
{
    [CommandLine.Option('c', "conf", HelpText = "Path to the configuration file", SetName = "fileConf")]
    public string? ConfigurationFilePath { get; init; }

    [CommandLine.Option('s', "--startcli", HelpText = "Does the component start with the CLI", Default = false)]
    public bool StartWithCLI { get; init; }

    [CommandLine.Option('i', "nodeid", HelpText = "Component's node id", SetName = "explicitConf")]
    public string NodeId { get; init; } = Guid.NewGuid().ToString();

    [CommandLine.Option('p', "port", HelpText = "Listening port for the component", SetName = "explicitConf")]
    public int ListenPort { get; init; }

    [CommandLine.Option('t', "timeout", HelpText = "Message timeout in milliseconds", SetName = "explicitConf")]
    public int TimeoutMs { get; init; }

    [CommandLine.Text.Usage(ApplicationAlias = "mediator")]
    public static IEnumerable<Example> Examples
    {
        get
        {
            return new List<Example>() {
            new Example("Start with configuration file", new CommandLineOptions { ConfigurationFilePath = "./configuration.json" }),
            new Example("Start with explicit arguments", new CommandLineOptions { NodeId = "SomeMediatorNode", ListenPort = 10000, TimeoutMs = 5000 })
            };
        }
    }
}

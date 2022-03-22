using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Janus.Communication.Tests.TestFixtures;

public class CommunicationNodeTestFixture
{
    public IServiceProvider ServiceProvider { get; private set; }

    private readonly IConfiguration _configuration;

    public IReadOnlyDictionary<string, CommunicationNodeOptions> MaskCommunicationNodeOptions => _maskCommunicationNodeOptions;
    public IReadOnlyDictionary<string, CommunicationNodeOptions> MediatorCommunicationNodeOptions => _mediatorCommunicationNodeOptions;
    public IReadOnlyDictionary<string, CommunicationNodeOptions> WrapperCommunicationNodeOptions => _wrapperCommunicationNodeOptions;

    private readonly Dictionary<string, CommunicationNodeOptions> _maskCommunicationNodeOptions;
    private readonly Dictionary<string, CommunicationNodeOptions> _mediatorCommunicationNodeOptions;
    private readonly Dictionary<string, CommunicationNodeOptions> _wrapperCommunicationNodeOptions;

    public CommunicationNodeTestFixture()
    {
        _maskCommunicationNodeOptions = new Dictionary<string, CommunicationNodeOptions>();
        _mediatorCommunicationNodeOptions = new Dictionary<string, CommunicationNodeOptions>();
        _wrapperCommunicationNodeOptions = new Dictionary<string, CommunicationNodeOptions>();

        _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testsettings.json", optional: false, reloadOnChange: true)
                .Build();

        LoadNodeOptions(_configuration);

        var services = new ServiceCollection();

        ServiceProvider = services.BuildServiceProvider();
    }

    private void LoadNodeOptions(IConfiguration configuration)
    {
        configuration.GetChildren()
                     .ToList()
                     .ForEach(section =>
                     {
                         var options = section.ToCommunicationNodeOptions();
                         var sectionKey = section.Key.ToLower();
                         if (sectionKey.Contains("mask"))
                         {
                             _maskCommunicationNodeOptions.Add(options.NodeId, options);
                         }
                         if (sectionKey.Contains("mediator"))
                         {
                             _mediatorCommunicationNodeOptions.Add(options.NodeId, options);   
                         }
                         if (sectionKey.Contains("wrapper"))
                         {
                             _wrapperCommunicationNodeOptions.Add(options.NodeId, options); 
                         }
                     });
    }
}

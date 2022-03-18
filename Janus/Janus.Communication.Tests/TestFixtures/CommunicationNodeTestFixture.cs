using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Janus.Communication.Tests.TestFixtures;

public class CommunicationNodeTestFixture
{
    public IServiceProvider ServiceProvider { get; private set; }

    private readonly IConfiguration _configuration;

    public IReadOnlyDictionary<string, MaskCommunicationNode> MaskCommunicationNodes => _maskCommunicationNodes;
    public IReadOnlyDictionary<string, MediatorCommunicationNode> MediatorCommunicationNodes => _mediatorCommunicationNodes;
    public IReadOnlyDictionary<string, WrapperCommunicationNode> WrapperCommunicationNodes => _wrapperCommunicationNodes;

    private readonly Dictionary<string, MaskCommunicationNode> _maskCommunicationNodes;
    private readonly Dictionary<string, MediatorCommunicationNode> _mediatorCommunicationNodes;
    private readonly Dictionary<string, WrapperCommunicationNode> _wrapperCommunicationNodes;

    public CommunicationNodeTestFixture()
    {
        _maskCommunicationNodes = new Dictionary<string, MaskCommunicationNode>();
        _mediatorCommunicationNodes = new Dictionary<string, MediatorCommunicationNode>();
        _wrapperCommunicationNodes = new Dictionary<string, WrapperCommunicationNode>();

        _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("nodeTests.appsettings.json", optional: false, reloadOnChange: true)
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
                             _maskCommunicationNodes.Add(options.Id, CommunicationNodes.CreateTcpMaskCommunicationNode(options));
                         }
                         if (sectionKey.Contains("mediator"))
                         {
                             _mediatorCommunicationNodes.Add(options.Id, CommunicationNodes.CreateTcpMediatorCommunicationNode(options));   
                         }
                         if (sectionKey.Contains("wrapper"))
                         {
                             _wrapperCommunicationNodes.Add(options.Id, CommunicationNodes.CreateTcpWrapperCommunicationNode(options)); 
                         }
                     });
    }
}

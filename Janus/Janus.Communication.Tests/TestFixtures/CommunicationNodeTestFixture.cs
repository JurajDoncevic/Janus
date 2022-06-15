using Janus.Communication.NetworkAdapters.Tcp;
using Janus.Communication.Nodes.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Janus.Communication.Tests.TestFixtures;

public class CommunicationNodeTestFixture
{
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
                .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
                .Build();


        var options = LoadCommunicationNodeOptions(_configuration);

        _maskCommunicationNodeOptions = options.maskNodeOptions.ToDictionary(o => o.NodeId, o => o);
        _mediatorCommunicationNodeOptions = options.mediatorNodeOptions.ToDictionary(o => o.NodeId, o => o);
        _wrapperCommunicationNodeOptions = options.wrapperNodeOptions.ToDictionary(o => o.NodeId, o => o);
    }

    public MaskCommunicationNode GetMaskCommunicationNode(string nodeId) 
        => CommunicationNodes.CreateTcpMaskCommunicationNode(
            _maskCommunicationNodeOptions[nodeId],
            new Janus.Utils.Logging.LoggerAdapter<MaskCommunicationNode>(_configuration),
            new Janus.Utils.Logging.LoggerAdapter<MaskNetworkAdapter>(_configuration)
            );

    public MediatorCommunicationNode GetMediatorCommunicationNode(string nodeId) 
        => CommunicationNodes.CreateTcpMediatorCommunicationNode(
            _mediatorCommunicationNodeOptions[nodeId],
            new Janus.Utils.Logging.LoggerAdapter<MediatorCommunicationNode>(_configuration),
            new Janus.Utils.Logging.LoggerAdapter<MediatorNetworkAdapter>(_configuration)
            );

    public WrapperCommunicationNode GetWrapperCommunicationNode(string nodeId) 
        => CommunicationNodes.CreateTcpWrapperCommunicationNode(
            _wrapperCommunicationNodeOptions[nodeId],
            new Janus.Utils.Logging.LoggerAdapter<WrapperCommunicationNode>(_configuration),
            new Janus.Utils.Logging.LoggerAdapter<WrapperNetworkAdapter>(_configuration)
            );

    private (
        IEnumerable<CommunicationNodeOptions> maskNodeOptions, 
        IEnumerable<CommunicationNodeOptions> mediatorNodeOptions, 
        IEnumerable<CommunicationNodeOptions> wrapperNodeOptions)
        LoadCommunicationNodeOptions(IConfiguration configuration)
    {
        return (
            configuration.GetChildren()
                         .ToList()
                         .SingleOrDefault(section => section.Key.Equals("Nodes")) ?
                         .GetChildren()
                         .SingleOrDefault(section => section.Key.Equals("Masks")) ?
                         .GetChildren()
                         .Select(maskSection => maskSection.GetCommunicationNodeOptions())
                         .ToList() ?? new List<CommunicationNodeOptions>(),
            configuration.GetChildren()
                         .ToList()
                         .SingleOrDefault(section => section.Key.Equals("Nodes"))?
                         .GetChildren()
                         .SingleOrDefault(section => section.Key.Equals("Mediators")) ?
                         .GetChildren()
                         .Select(maskSection => maskSection.GetCommunicationNodeOptions())
                         .ToList() ?? new List<CommunicationNodeOptions>(),
            configuration.GetChildren()
                         .ToList()
                         .SingleOrDefault(section => section.Key.Equals("Nodes"))?
                         .GetChildren()
                         .SingleOrDefault(section => section.Key.Equals("Wrappers")) ?
                         .GetChildren()
                         .Select(maskSection => maskSection.GetCommunicationNodeOptions())
                         .ToList() ?? new List<CommunicationNodeOptions>()
                     );
                     
    }
}

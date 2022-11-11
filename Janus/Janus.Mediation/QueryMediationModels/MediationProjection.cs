namespace Janus.Mediation.QueryMediationModels;
public class MediationProjection
{
    private readonly HashSet<string> _mediatedAttributeIds;
    private readonly HashSet<string> _sourceAttributeIds;
    private readonly Dictionary<string, string> _mediatedAttributeMappings;

    public MediationProjection(HashSet<string> mediatedAttributeIds, HashSet<string> sourceAttributeIds, Dictionary<string, string> mediatedAttributeMappings)
    {
        _mediatedAttributeIds = mediatedAttributeIds;
        _sourceAttributeIds = sourceAttributeIds;
        _mediatedAttributeMappings = mediatedAttributeMappings;
    }

    public IReadOnlySet<string> MediatedAttributeIds => _mediatedAttributeIds;

    public IReadOnlySet<string> SourceAttributeIds => _sourceAttributeIds;

    public IReadOnlyDictionary<string, string> MediatedAttributeMappings => _mediatedAttributeMappings;
}

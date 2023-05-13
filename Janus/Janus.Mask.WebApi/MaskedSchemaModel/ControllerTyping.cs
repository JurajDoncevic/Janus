using Janus.Base;
using Janus.Commons.SchemaModels;

namespace Janus.Mask.WebApi.MaskedSchemaModel;
public class ControllerTyping
{
    private readonly string _controllerClassName;
    private readonly string _route;
    private readonly string _routePrefix;
    private readonly Type _idPropertyType;
    private readonly DtoTyping _getDto;
    private readonly Option<DtoTyping> _postDto;
    private readonly IEnumerable<DtoTyping> _putDtos;
    private readonly bool _enablesDelete;
    private readonly TableauId _targetTableauId;
    private readonly AttributeId _identityAttributeId;

    public ControllerTyping(string controllerClassName,
                            string route,
                            string routePrefix,
                            Type idPropertyType,
                            DtoTyping getDto,
                            Option<DtoTyping> postDto,
                            IEnumerable<DtoTyping> putDtos,
                            bool enablesDelete,
                            TableauId targetTableauId,
                            AttributeId identityAttributeId)
    {
        _controllerClassName = !controllerClassName.EndsWith("Controller") ? controllerClassName + "Controller" : controllerClassName;
        _route = route;
        _routePrefix = routePrefix;
        _idPropertyType = idPropertyType ?? throw new ArgumentNullException(nameof(idPropertyType));
        _getDto = getDto ?? throw new ArgumentNullException(nameof(getDto));
        _postDto = postDto;
        _putDtos = putDtos ?? throw new ArgumentNullException(nameof(putDtos));
        _enablesDelete = enablesDelete;
        _targetTableauId = targetTableauId ?? throw new ArgumentNullException(nameof(targetTableauId));
        _identityAttributeId = identityAttributeId ?? throw new ArgumentNullException(nameof(identityAttributeId));
    }

    public string ControllerClassName => _controllerClassName;

    public string Route => _route;

    public string RoutePrefix => _routePrefix;

    public Type IdPropertyType => _idPropertyType;

    public DtoTyping GetDto => _getDto;

    public Option<DtoTyping> PostDto => _postDto;

    public IEnumerable<DtoTyping> PutDtos => _putDtos;

    public TableauId TargetTableauId => _targetTableauId;

    public AttributeId IdentityAttributeId => _identityAttributeId;

    public bool EnablesDelete => _enablesDelete;
}

using FunctionalExtensions.Base;
using Janus.Commons.SchemaModels;

namespace Janus.Mask.WebApi.MaskedSchemaModel;
public class ControllerTyping
{
    private readonly string _controllerName;
    private readonly string _routePrefix;
    private readonly Type _idPropertyType;
    private readonly DtoTyping _getDto;
    private readonly Option<DtoTyping> _postDto;
    private readonly IEnumerable<DtoTyping> _putDtos;
    private readonly bool _enablesDelete;
    private readonly TableauId _targetTableauId;
    private readonly AttributeId _identityAttributeId;

    public ControllerTyping(string controllerName,
                            string routePrefix,
                            Type idPropertyType,
                            DtoTyping getDto,
                            Option<DtoTyping> postDto,
                            IEnumerable<DtoTyping> putDtos,
                            bool enablesDelete,
                            TableauId targetTableauId,
                            AttributeId identityAttributeId)
    {
        _controllerName = controllerName;
        _routePrefix = routePrefix;
        _idPropertyType = idPropertyType;
        _getDto = getDto;
        _postDto = postDto;
        _putDtos = putDtos;
        _enablesDelete = enablesDelete;
        _targetTableauId = targetTableauId;
        _identityAttributeId = identityAttributeId;
    }

    public string ControllerName => _controllerName;

    public string RoutePrefix => _routePrefix;

    public Type IdPropertyType => _idPropertyType;

    public DtoTyping GetDto => _getDto;

    public Option<DtoTyping> PostDto => _postDto;

    public IEnumerable<DtoTyping> PutDtos => _putDtos;

    public TableauId TargetTableauId => _targetTableauId;

    public AttributeId IdentityAttributeId => _identityAttributeId;

    public bool EnablesDelete => _enablesDelete;
}

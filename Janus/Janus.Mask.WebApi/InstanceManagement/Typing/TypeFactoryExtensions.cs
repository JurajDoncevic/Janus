using Janus.Mask.WebApi.InstanceManagement.Templates;
using Janus.Mask.WebApi.MaskedSchemaModel;

namespace Janus.Mask.WebApi.InstanceManagement.Typing;
public static class TypeFactoryExtensions
{
    public static Type GenerateDtoType(this DtoTyping dtoTyping, TypeFactory typeFactory)
    {
        var generatedType = typeFactory.CreateDtoType(dtoTyping, "Janus.Mask.WebApi.InstanceManagement.Dynamic", typeof(BaseDto));
        return generatedType;
    }
}

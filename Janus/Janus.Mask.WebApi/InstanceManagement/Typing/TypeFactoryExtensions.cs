using Janus.Mask.WebApi.InstanceManagement.Templates;

namespace Janus.Mask.WebApi.InstanceManagement.Typing;
public static class TypeFactoryExtensions
{
    public static Type GenerateType(this DtoTyping dtoTyping, TypeFactory typeFactory)
    {
        var generatedType = typeFactory.CreateDtoType(dtoTyping, "JanusGenericMask.InstanceManagement.Web.Dynamic", typeof(BaseDto));
        return generatedType;
    }
}

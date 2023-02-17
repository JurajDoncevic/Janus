using FunctionalExtensions.Base;
using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using Janus.Mask.WebApi.InstanceManagement.Typing;

namespace Janus.Mask.WebApi.Translation;
internal class WebApiSchemaTranslation
{
    internal static IEnumerable<ControllerTyping> GetControllerTypings(DataSource dataSourceSchema)
    {
        IEnumerable<ControllerTyping> controllerTypings = Enumerable.Empty<ControllerTyping>();
        if (dataSourceSchema == null)
        {
            return controllerTypings;
        }
        foreach (var schema in dataSourceSchema.Schemas)
        {
            string controllerNamePrefix = $"{schema.Name}";

            foreach (var tableau in schema.Tableaus)
            {
                var idPropertyType = TypeMappings.MapToType(
                                        tableau.Attributes.FirstOrDefault(attr => attr.IsIdentity)?.DataType ??
                                        tableau.Attributes.MinBy(attr => attr.Ordinal)?.DataType ??
                                        throw new Exception("Couldn't determine ID type"));


                var defaultUpdateSet = tableau.UpdateSets.FirstOrDefault(us => tableau.AttributeNames.All(attrName => us.AttributeNames.Contains(attrName)));
                var postDto =
                    defaultUpdateSet is not null
                    ? Option<DtoTyping>.Some(
                        new DtoTyping(
                            $"{tableau.Name}_Post",
                            idPropertyType,
                            defaultUpdateSet.AttributeNames.ToDictionary( // .Where(attrName => !tableau[attrName].IsIdentity)
                                attrName => attrName,
                                attrName => TypeMappings.MapToType(tableau[attrName].DataType)
                                )))
                    : Option<DtoTyping>.None;

                var getDto =
                    new DtoTyping(
                        $"{tableau.Name}_Get",
                        idPropertyType,
                        tableau.Attributes.ToDictionary(
                            attr => attr.Name,
                            attr => TypeMappings.MapToType(attr.DataType)
                            ));

                var putDtos =
                    tableau.UpdateSets
                        .Mapi((idx, us) => new DtoTyping(
                            $"{tableau.Name}_Put_{idx}",
                            idPropertyType,
                            us.AttributeNames.Where(attrName => !tableau[attrName].IsIdentity).ToDictionary(
                                    attrName => attrName,
                                    attrName => TypeMappings.MapToType(tableau[attrName].DataType)
                                    )
                            ));


                var controllerTyping =
                        new ControllerTyping(
                            tableau.Name,
                            schema.Name,
                            idPropertyType,
                            getDto,
                            postDto,
                            putDtos,
                            tableau.Id,
                            tableau.Attributes.First(attr => attr.IsIdentity).Id // exception should already be thrown above
                            );


                controllerTypings = controllerTypings.Append(controllerTyping);
            }
        }

        return controllerTypings;
    }
}

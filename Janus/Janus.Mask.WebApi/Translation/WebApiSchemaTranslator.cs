using Janus.Base;
using Janus.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using Janus.Mask.Translation;
using Janus.Mask.WebApi.MaskedSchemaModel;

namespace Janus.Mask.WebApi.Translation;

public class WebApiSchemaTranslator : IMaskSchemaTranslator<WebApiTyping>
{
    public Result<WebApiTyping> Translate(DataSource dataSource)
        => Results.AsResult(() =>
        {
            IEnumerable<ControllerTyping> controllerTypings = Enumerable.Empty<ControllerTyping>();
            if (dataSource == null)
            {
                return new WebApiTyping(controllerTypings);
            }
            foreach (var schema in dataSource.Schemas)
            {
                string controllerNamePrefix = $"{schema.Name}_";

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
                                    ),
                                $"{tableau.Schema.Name}_"))
                        : Option<DtoTyping>.None;

                    var getDto =
                        new DtoTyping(
                            $"{tableau.Name}_Get",
                            idPropertyType,
                            tableau.Attributes.ToDictionary(
                                attr => attr.Name,
                                attr => TypeMappings.MapToType(attr.DataType)
                                ),
                            $"{tableau.Schema.Name}_"
                            );

                    var putDtos =
                        tableau.UpdateSets
                            .Mapi((idx, us) => new DtoTyping(
                                $"{tableau.Name}_Put_{idx}",
                                idPropertyType,
                                us.AttributeNames.Where(attrName => !tableau[attrName].IsIdentity).ToDictionary(
                                        attrName => attrName,
                                        attrName => TypeMappings.MapToType(tableau[attrName].DataType)
                                        ),
                                $"{tableau.Schema.Name}_"
                                ));

                    bool enablesDelete = defaultUpdateSet is not null;

                    var controllerTyping =
                            new ControllerTyping(
                                controllerNamePrefix + tableau.Name,
                                tableau.Name,
                                schema.Name,
                                idPropertyType,
                                getDto,
                                postDto,
                                putDtos,
                                enablesDelete,
                                tableau.Id,
                                tableau.Attributes.First(attr => attr.IsIdentity).Id // exception should already be thrown above
                                );


                    controllerTypings = controllerTypings.Append(controllerTyping);
                }
            }

            return new WebApiTyping(controllerTypings);
        });
}

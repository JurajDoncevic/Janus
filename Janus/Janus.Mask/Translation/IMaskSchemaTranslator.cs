using Janus.Commons.SchemaModels;
using Janus.Components.Translation;
using Janus.Mask.MaskedSchemaModel;

namespace Janus.Mask.Translation;
public interface IMaskSchemaTranslator<TMaskedSchema> : ISchemaTranslator<DataSource, TMaskedSchema>
    where TMaskedSchema : MaskedDataSource
{
}

using FunctionalExtensions.Base.Results;
using Janus.Wrapper.SchemaInferrence.Model;

namespace Janus.Wrapper.SchemaInferrence;
public interface ISchemaModelProvider
{
    public Result<DataSourceInfo> GetDataSource();
    public Result<IEnumerable<SchemaInfo>> GetSchemas();
    public Result SchemaExists(string schemaName);
    public Result<IEnumerable<TableauInfo>> GetTableaus(string schemaName);
    public Result TableauExists(string schemaName, string tableauName);
    public Result<IEnumerable<AttributeInfo>> GetAttributes(string schemaName, string tableauName);
    public Result AttributeExists(string schemaName, string tableauName, string attributeName);
}

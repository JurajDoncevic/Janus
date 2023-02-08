using FunctionalExtensions.Base.Resulting;
using Janus.Wrapper.SchemaInference.Model;

namespace Janus.Wrapper.SchemaInference;

/// <summary>
/// Describes a schema model provider
/// </summary>
public interface ISchemaModelProvider
{
    /// <summary>
    /// Gets the information about the data source
    /// </summary>
    /// <returns></returns>
    public Result<DataSourceInfo> GetDataSource();
    /// <summary>
    /// Gets all expected schemas' information in the data source
    /// </summary>
    /// <returns></returns>
    public Result<IEnumerable<SchemaInfo>> GetSchemas();
    /// <summary>
    /// Checks if a schema with given name is expected to exist in the data source
    /// </summary>
    /// <param name="schemaName">Expected schema name</param>
    /// <returns></returns>
    public Result SchemaExists(string schemaName);
    /// <summary>
    /// Gets all expected tableaus' information in an expected schema
    /// </summary>
    /// <param name="schemaName">Expected schema name</param>
    /// <returns></returns>
    public Result<IEnumerable<TableauInfo>> GetTableaus(string schemaName);
    /// <summary>
    /// Checks if an expected tableau with given name and expected schema name exists in the data source
    /// </summary>
    /// <param name="schemaName"></param>
    /// <param name="tableauName"></param>
    /// <returns></returns>
    public Result TableauExists(string schemaName, string tableauName);
    /// <summary>
    /// Gets all expected attributes' information from an expected tableau from in an expected schema
    /// </summary>
    /// <param name="schemaName"></param>
    /// <param name="tableauName"></param>
    /// <returns></returns>
    public Result<IEnumerable<AttributeInfo>> GetAttributes(string schemaName, string tableauName);
    /// <summary>
    /// Checks if an expected attribute exists under an expected tableau and schema
    /// </summary>
    /// <param name="schemaName"></param>
    /// <param name="tableauName"></param>
    /// <param name="attributeName"></param>
    /// <returns></returns>
    public Result AttributeExists(string schemaName, string tableauName, string attributeName);
}

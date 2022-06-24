using Janus.Commons.SchemaModels;
using Janus.Wrapper.Core.SchemaInferrence.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Core.SchemaInferrence;
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

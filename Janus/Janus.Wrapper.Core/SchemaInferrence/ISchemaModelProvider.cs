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
    Result<DataSourceInfo> GetDataSource();
    Result<IEnumerable<SchemaInfo>> GetSchemasInDataSource();
    Result<IEnumerable<TableauInfo>> GetTableausInSchema(string schemaName);
    Result<IEnumerable<AttributeInfo>> GetAttributesInTableau(string tableauName);
}
